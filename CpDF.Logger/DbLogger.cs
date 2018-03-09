﻿using CpDF.Logger.Events;
using CpDF.Logger.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CpDF.Logger
{
    public class DbLogger
    {
        private List<LogData> m_LogDataList { get; set; }
        public DbLogger(string logPath, string logFileName, string logId = "NoId", int debugLevel = DebugLevelConstants.Medium, bool onlyConsoleOutput = false)
        {
            m_LogDataList = new List<LogData>();

            Settings.mainThreadId = Thread.CurrentThread.ManagedThreadId;
            Settings.LogId = logId;
            Settings.DebugLevel = debugLevel;

            Settings.LogFile = Path.Combine(logPath, logFileName + ".log");
            if(!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            SLLogEvents.ShowLogFile += OnShowLogFile;
        }

        private void OnShowLogFile(SLLogEventArgs args)
        {
            if (!File.Exists(Settings.LogFile)) return;
            Process.Start(Settings.LogFile);
        }

        public LogData WriteInfo(LogData data, bool onlyToolTipp = false, int debugLevel = DebugLevelConstants.Unknow, bool onlyReturnLogData = false)
        {
            if (Settings.DebugLevel < debugLevel && debugLevel != DebugLevelConstants.Unknow) return data;
            if (Settings.DebugLevel < DebugLevelConstants.High) return data;

            data.Type = LogType.Info;
            data.ExDate = DateTime.Now;

            if (onlyReturnLogData) return data;

            CreateBallonTipp(data);
            if (!onlyToolTipp)
            {
                m_LogDataList.Add(data);
                LogToFile();
            }
            return data;
        }

        public LogData WriteWarnng(LogData data, int debugLevel = DebugLevelConstants.Unknow, bool onlyReturnLogData = false)
        {
            if (Settings.DebugLevel < debugLevel && debugLevel != DebugLevelConstants.Unknow) return data;
            if (Settings.DebugLevel < DebugLevelConstants.Medium) return data;

            data.Type = LogType.Warning;
            data.ExDate = DateTime.Now;

            if (onlyReturnLogData) return data;

            m_LogDataList.Add(data);
            CreateBallonTipp(data);

            LogToFile();
            return data;
        }

        public LogData WriteError(LogData data, int debugLevel = DebugLevelConstants.Unknow, bool onlyReturnLogData = false)
        {
            if (Settings.DebugLevel < debugLevel && debugLevel != DebugLevelConstants.Unknow) return data;
            if (Settings.DebugLevel < DebugLevelConstants.Low) return data;

            data.Type = LogType.Error;
            data.ExDate = DateTime.Now;

            if (data.Ex != null)
            {
                data.StackTrace = data.Ex.StackTrace;
                data.Message = data.Ex.Message;

                // Get stack trace for the exception with source file information
                var st = new StackTrace(data.Ex, true);
                if (st != null)
                {
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    if (frame != null) data.LineNumber = frame.GetFileLineNumber();
                }
            }

            if (onlyReturnLogData) return data;

            m_LogDataList.Add(data);
            CreateBallonTipp(data);

            LogToFile();
            return data;
        }

        private void CreateBallonTipp(LogData data)
        {
            if (Settings.OnlyConsoleOutput) return;

            var title = string.Empty;
            if (Settings.IsMainThread)
                SLLogEvents.FireShowBallonTipp(new SLLogEventArgs { Type = data.Type, Titel = "DatabaseFactory " + data.Type.ToString() + "!", LogMessage = data.Message });
        }

        private void LogToFile()
        {
            WriteToFile();
        }

        public string GetLogText(LogData logEntry)
        {
            switch (logEntry.Type)
            {
                case LogType.Info:
                    return string.Format(@"[Info]{1} => {2}: {0}Function: {3} {0}Message: {4}{5}{0}{0}",
                            Environment.NewLine, Settings.LogId, logEntry.ExDate.ToString() + string.Format(" [{0}]", logEntry.ExDate.Millisecond), logEntry.FunctionName, logEntry.Message,
                            logEntry.LineNumber != 0
                                ? string.Format(" [Line: {0}]", logEntry.LineNumber)
                                : string.Empty);

                case LogType.Warning:
                    return string.Format(@"[Warning]{1} => {2}: {0}Function: {3} {0}Source: {4} {0}Message: {5}{6}{0}{0}",
                            Environment.NewLine, Settings.LogId, logEntry.ExDate.ToString() + string.Format(" [{0}]", logEntry.ExDate.Millisecond), logEntry.FunctionName, logEntry.Source, logEntry.Message, 
                            logEntry.LineNumber != 0 
                                ? string.Format(" [Line: {0}]", logEntry.LineNumber) 
                                : string.Empty);

                case LogType.Error:
                    return string.Format(@"[Error]{1} => {2}: {0}Function: {3} {0}Source: {4} {0}Message: {5} {0}StackTrace: {6} [Line: {7}]{0}{0}",
                            Environment.NewLine, Settings.LogId, logEntry.ExDate.ToString() + string.Format(" [{0}]", logEntry.ExDate.Millisecond), logEntry.FunctionName, logEntry.Source, logEntry.Message,
                            logEntry.StackTrace, logEntry.LineNumber);
            }
            return string.Empty;
        }

        private void WriteToFile()
        {
            foreach(var logEntry in m_LogDataList)
            {
                if (logEntry.IsInLogFile) continue;

                var line = string.Empty;
                try
                {
                    switch(logEntry.Type)
                    {
                        case LogType.Info:
                        case LogType.Warning:
                            line = GetLogText(logEntry);
                            WriteConsoleLog(line);
                            break;

                        case LogType.Error:
                            line = GetLogText(logEntry);
                            
                            WriteConsoleLog(line);
                            break;
                    }

                    WriteAsync(Settings.LogFile, line);
                    logEntry.IsInLogFile = true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error writing Log: " + ex.Message);
                    logEntry.IsInLogFile = false;
                }
            }
        }

        private bool m_FileLocked { get; set; }
        private void WriteAsync(string path, string line)
        {
            if (Settings.OnlyConsoleOutput) return;
            Task.Run(() =>
            {
                try
                {
                    while(m_FileLocked)
                    {
                        Thread.Sleep(100);
                    }
                    m_FileLocked = true;

                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(line);
                    }
                    m_FileLocked = false;
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error writing Log: " + ex.Message);
                }
            });           
        }

        private void WriteConsoleLog(string message)
        {
            Console.WriteLine(message);
        }
    }
}
