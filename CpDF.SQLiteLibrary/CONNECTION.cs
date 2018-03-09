using CpDF.Interface.Models;
using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace CpDF.SQLiteLibrary
{
    public class CONNECTION
    {
        public CONNECTION(DbConnectionData conData)
        {
            if (string.IsNullOrEmpty(conData.Name)) return;

            if(string.IsNullOrEmpty(conData.Path))
            {
                Settings.ConnectionString = string.Format("Data Source={0}", conData.Name);
            }
            else
            {
                Settings.ConnectionString = string.Format("Data Source={0}", Path.Combine(conData.Path, conData.Name));
            }

            //Set Addentional Settings
            Settings.ThrowExceptions = conData.ThrowExceptions;
        }

        public static SqliteConnection OpenCon()
        {
            var con = new SqliteConnection(Settings.ConnectionString);
            con.Open();
            return con;
        }

        public static void CloseCon(SqliteConnection con)
        {
            con.Close();
            con.Dispose();

            GC.Collect();
        }
    }
}
