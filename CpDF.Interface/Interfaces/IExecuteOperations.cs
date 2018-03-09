﻿using CpDF.Interface.Models;
using System.Collections.Generic;
using System.Data;

namespace CpDF.Interface.Interfaces
{
    public interface IExecuteOperations
    {
        int ExecuteNonQuery(List<string> sqlList);
        int ExecuteNonQuery(string sql);

        object ExecuteScalar(string sql);

        DataTable ExecuteReadTable(string sql);
        DataTable ExecuteReadTableSchema(string sql);

        string ExecuteReadTableName(string columnName);

        bool RenewTbl(string tableName, Dictionary<string, string> columns);
        bool RenewTbl(string tableName, List<ColumnData> columns);
    }
}
