﻿using CpDF.Interface;
using CpDF.Interface.Helpers;
using CpDF.Interface.Interfaces;
using CpDF.Interface.Models;
using CpDF.Logger.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace CpDF.SQLiteLibrary.Operations
{
    public class SQLiteInsert : IInsertOperations
    {
        SQLiteExecute m_Execute { get; set; }
        public SQLiteInsert()
        {
            m_Execute = new SQLiteExecute();
        }

        public bool CreateTable(string tableName, Dictionary<string, string> columns)
        {
            try
            {
                var colList = new List<ColumnData>();
                foreach(var col in columns)
                {
                    colList.Add(new ColumnData
                    {
                        Name = col.Key,
                        Type = col.Value
                    });
                }

                return CreateTable(tableName, colList);
            }
            catch(Exception ex)
            {
                SLLog.WriteError(new LogData
                {
                    Source = ToString(),
                    FunctionName = "CreateTable Error!",
                    Ex = ex,
                });
                return false;
            }
        }

        public bool CreateTable(string tableName, List<ColumnData> columns)
        {
            try
            {
                ColumnHelper.SetDefaultColumns(columns);

                var sql = ScriptHelper.GetCreateTableSql(tableName, columns);
                var result = m_Execute.ExecuteNonQuery(sql);

                if (result == -2) return false;
                return true;
            }
            catch (Exception ex)
            {
                SLLog.WriteError(new LogData
                {
                    Source = ToString(),
                    FunctionName = "CreateTable Error!",
                    Ex = ex,
                });
                return false;
            }
        }

        public bool CreateDatabase(string databaseName)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                SLLog.WriteError(new LogData
                {
                    Source = ToString(),
                    FunctionName = "CreateDatabase Error!",
                    Ex = ex,
                });
                return false;
            }
        }

        public bool InsertRow(string tableName, DataRow row, bool setInsertOn = true)
        {
            try
            {
                var colRowDict = new Dictionary<string, string>();
                foreach(DataColumn dc in row.Table.Columns)
                {
                    colRowDict.Add(dc.ColumnName, row[dc.ColumnName].ToString());
                }

                return InsertValue(tableName, colRowDict, setInsertOn);
            }
            catch(Exception ex)
            {
                SLLog.WriteError(new LogData
                {
                    Source = ToString(),
                    FunctionName = "InsertRow Error!",
                    Ex = ex,
                });
                return false;
            }
        }

        public bool InsertValue(string tableName, string columnName, string value, bool setInsertOn = true)
        {
            try
            {
                var colRowDict = new Dictionary<string, string>();
                colRowDict.Add(columnName, value);
                return InsertValue(tableName, colRowDict, setInsertOn);
            }
            catch(Exception ex)
            {
                SLLog.WriteError(new LogData
                {
                    Source = ToString(),
                    FunctionName = "InsertValue Error!",
                    Ex = ex,
                });
                return false;
            }
        }

        public bool InsertValue(string tableName, Dictionary<string, string> data, bool setInsertOn = true)
        {
            try
            {
                var sql = ScriptHelper.GetInsertSqlScript(tableName, data, setInsertOn);
                var result = m_Execute.ExecuteNonQuery(sql);

                if (result == -2) return false;
                return true;
            }
            catch(Exception ex)
            {
                SLLog.WriteError(new LogData
                {
                    Source = ToString(),
                    FunctionName = "InsertValue Error!",
                    Ex = ex,
                });
                return false;
            }
        }
    }
}
