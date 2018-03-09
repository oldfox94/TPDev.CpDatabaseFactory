using CpDF.Interface.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace CpDF.Interface.Helpers
{
    public static class ScriptHelper
    {
        public static string GetCreateTableSql(string tableName, List<ColumnData> columns)
        {
            var sql = string.Format(@"CREATE TABLE [{0}] (", tableName);

            int count = 0;
            foreach (var col in columns)
            {
                count++;

                sql += string.Format(@"[{0}] ", col.Name);
                foreach (var colSettings in columns)
                {
                    if (colSettings.Name == col.Name)
                    {
                        sql += colSettings.Type;
                        if (!string.IsNullOrEmpty(colSettings.DefaultValue))
                        {
                            sql += string.Format(@" DEFAULT '{0}'", colSettings.DefaultValue);
                        }
                        break;
                    }
                }

                if (count != columns.Count)
                    sql += ", ";
                else
                    sql += ")";
            }

            return sql;
        }

        public static string GetMySQLCreateTableSql(string tableName, List<ColumnData> columns)
        {
            var sql = string.Format(@"CREATE TABLE {0} (", tableName);

            int count = 0;
            foreach(var col in columns)
            {
                count++;

                sql += string.Format(@"{0} ", col.Name);
                foreach(var colSettings in columns)
                {
                    if(colSettings.Name == col.Name)
                    {
                        sql += colSettings.Type;
                        if(!string.IsNullOrEmpty(colSettings.DefaultValue))
                        {
                            sql += string.Format(@" DEFAULT '{0}'", colSettings.DefaultValue);
                        }
                        break;
                    }
                }

                if (count != columns.Count)
                    sql += ", ";
                else
                    sql += ")";
            }

            return sql;
        }

        public static string GetInsertSqlScript(string tableName, Dictionary<string, string> data, bool setInsertOn = true)
        {
            var sql = string.Empty;
            string columns = "";
            string values = "";

            ColumnHelper.SetDefaultColumnValues(data, setInsertOn);

            foreach(KeyValuePair<string, string> val in data)
            {
                columns += string.Format(@" {0},", val.Key.ToString());
                values += string.Format(@" '{0}',", ConvertionHelper.CleanStringForSQL(val.Value));
            }
            columns = columns.Substring(0, columns.Length -1);
            values = values.Substring(0, values.Length -1);

            sql = string.Format(@"INSERT INTO {0}({1}) values({2})", tableName, columns, values);
            return sql;
        }

        public static string GetUpdateSqlScript(DataTable tbl, bool setModifyOn = true)
        {
            var sql = string.Empty;
            foreach(DataRow dr in tbl.Rows)
            {
                if (dr.RowState != DataRowState.Modified) continue;

                var data = new Dictionary<string, string>();
                foreach (DataColumn col in tbl.Columns)
                {
                    data.Add(col.ColumnName, dr[col.ColumnName].ToString());
                }
                sql += GetUpdateSqlFieldScript(data, setModifyOn) + ", ";
            }
            sql = sql.Substring(0, sql.Length - 1);

            return sql;
        }
        public static string GetUpdateSqlScript(string tableName, Dictionary<string, string> data, bool setModifyOn = true)
        {
            var columnValuePair = GetUpdateSqlFieldScript(data, setModifyOn);
            var sql = string.Format(@"UPDATE {0} SET {1}", tableName, columnValuePair);
            return sql;
        }

        private static string GetUpdateSqlFieldScript(Dictionary<string, string> data, bool setModifyOn = true)
        {
            string columnValuePair = "";

            ColumnHelper.SetDefaultColumnValues(data);
            if (setModifyOn)
            {
                if (data.ContainsKey(DbCIC.ModifyOn)) data[DbCIC.ModifyOn] = DateTime.Now.ToString();
            }

            foreach (KeyValuePair<string, string> val in data)
            {
                columnValuePair += string.Format(@" {0} = '{1}',", val.Key.ToString(), ConvertionHelper.CleanStringForSQL(val.Value));
            }
            columnValuePair = columnValuePair.Substring(0, columnValuePair.Length - 1);
            return columnValuePair;
        }
    }
}
