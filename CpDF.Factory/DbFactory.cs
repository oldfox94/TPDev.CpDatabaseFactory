using CpDF.Interface;
using CpDF.Interface.Interfaces;
using CpDF.Interface.Models;
using CpDF.Logger;
using CpDF.Logger.Models;
using CpDF.SQLiteLibrary.Operations;
using System;

namespace CpDF.Factory
{
    public class DbFactory
    {
        public IInsertOperations Insert { get; set; }
        public IUpdateOperations Update { get; set; }
        public IDeleteOperations Delete { get; set; }
        public ICheckOperations Check { get; set; }
        public IGetOperations Get { get; set; }
        public IExecuteOperations Execute { get; set; }


        public DbFactory(DbType type, DbConnectionData data)
        {
            DbFactorySettings.Type = type;
            switch(type)
            {
                case DbType.SQL:
                    //new SQLLibrary.CONNECTION(data);
                    break;

                case DbType.SQLite:
                    new SQLiteLibrary.CONNECTION(data);
                    break;

                case DbType.MySQL:
                    //new MySQLLibrary.CONNECTION(data);
                    break;

                case DbType.Oracle:
                    //new OracleLibrary.CONNECTION(data);
                    break;
            }

            DbFactorySettings.Factory = this;

            Insert = GetInsertService();
            Update = GetUpdateService();
            Delete = GetDeleteService();
            Check = GetCheckService();
            Get = GetGetService();
            Execute = GetExecuteService();
        }

        public void InitLogger(string logFileName, string logPath = null, string logId = "", int debugLevel = DebugLevelConstants.Medium, bool onlyConsoleOutput = false)
        {
            if (string.IsNullOrEmpty(logPath))
                logPath = Environment.CurrentDirectory;

            if (string.IsNullOrEmpty(logId)) logId = DbFactorySettings.Type.ToString();
            SLLog.Logger = new DbLogger(logPath, logFileName, logId, debugLevel, onlyConsoleOutput);
            SLLog.WriteInfo("InitLogger", "Logger successfully initialized!");
        }

        public IInsertOperations GetInsertService()
        {
            switch (DbFactorySettings.Type)
            {
                case DbType.SQL:
                    //return new SQLInsert();
                    return null;

                case DbType.SQLite:
                    return new SQLiteInsert();

                case DbType.MySQL:
                    //return new MySQLInsert();
                    return null;

                case DbType.Oracle:
                    //return new OraInsert();
                    return null;
            }

            return null;
        }

        public IUpdateOperations GetUpdateService()
        {
            switch (DbFactorySettings.Type)
            {
                case DbType.SQL:
                    //return new SQLUpdate();
                    return null;

                case DbType.SQLite:
                    return new SQLiteUpdate();

                case DbType.MySQL:
                    //return new MySQLUpdate();
                    return null;

                case DbType.Oracle:
                    //return new OraUpdate();
                    return null;
            }

            return null;
        }

        public IDeleteOperations GetDeleteService()
        {
            switch (DbFactorySettings.Type)
            {
                case DbType.SQL:
                    //return new SQLDelete();
                    return null;

                case DbType.SQLite:
                    return new SQLiteDelete();

                case DbType.MySQL:
                    //return new MySQLDelete();
                    return null;

                case DbType.Oracle:
                    //return new OraDelete();
                    return null;
            }

            return null;
        }

        public ICheckOperations GetCheckService()
        {
            switch(DbFactorySettings.Type)
            {
                case DbType.SQL:
                    //return new SQLCheck();
                    return null;

                case DbType.SQLite:
                    return new SQLiteCheck();

                case DbType.MySQL:
                    //return new MySQLCheck();
                    return null;

                case DbType.Oracle:
                    //return new OraCheck();
                    return null;
            }

            return null;
        }

        public IGetOperations GetGetService()
        {
            switch(DbFactorySettings.Type)
            {
                case DbType.SQL:
                    //return new SQLGet();
                    return null;

                case DbType.SQLite:
                    return new SQLiteGet();

                case DbType.MySQL:
                    //return new MySQLGet();
                    return null;

                case DbType.Oracle:
                    //return new OraGet();
                    return null;
            }

            return null;
        }

        public IExecuteOperations GetExecuteService()
        {
            switch (DbFactorySettings.Type)
            {
                case DbType.SQL:
                    //return new SQLExecute();
                    return null;

                case DbType.SQLite:
                    return new SQLiteExecute();

                case DbType.MySQL:
                    //return new MySQLExecute();
                    return null;

                case DbType.Oracle:
                    //return new OraExecute();
                    return null;
            }

            return null;
        }
    }
}
