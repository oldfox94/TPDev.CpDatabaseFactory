﻿using CpDF.Logger;
using CpDF.Logger.Models;
using System;

namespace CpDF.Interface
{
    public class SLLog
    {
        public static DbLogger Logger { get; set; }

        public static LogData WriteInfo(string function, string message, bool onlyBallonTipp = false, int debugLevel = DebugLevelConstants.Unknow, string logId = "", int initialDebugLevel = DebugLevelConstants.Medium, bool onlyConsoleOutput = false, bool onlyReturnLogData = false)
        {
            if (Logger == null)
                Logger = new DbLogger(Environment.CurrentDirectory, "DbFactory", logId, initialDebugLevel, onlyConsoleOutput);

            var data = new LogData
            {
                FunctionName = function,
                Message = message,
            };

            return Logger.WriteInfo(data, onlyBallonTipp, debugLevel, onlyReturnLogData);
        }

        public static LogData WriteWarning(string function, string source, string message, int debugLevel = DebugLevelConstants.Unknow, string logId = "", int initialDebugLevel = DebugLevelConstants.Medium, bool onlyConsoleOutput = false, bool onlyReturnLogData  = false)
        {
            if (Logger == null)
                Logger = new DbLogger(Environment.CurrentDirectory, "DbFactory", logId, initialDebugLevel, onlyConsoleOutput);

            var data = new LogData
            {
                FunctionName = function,
                Source = source,
                Message = message,
            };

            return Logger.WriteWarnng(data, debugLevel, onlyReturnLogData);
        }

        public static LogData WriteError(LogData data, int debugLevel = DebugLevelConstants.Unknow, string logId = "", int initialDebugLevel = DebugLevelConstants.Medium, bool onlyConsoleOutput = false, bool onlyReturnLogData = false)
        {
            if (Logger == null)
                Logger = new DbLogger(Environment.CurrentDirectory, "DbFactory", logId, initialDebugLevel, onlyConsoleOutput);

            return Logger.WriteError(data, debugLevel, onlyReturnLogData);
        }
    }
}
