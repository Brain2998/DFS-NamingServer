using System;
using System.Data.SQLite;
//using Mono.Data.Sqlite;
using System.IO;
using System.Collections.Generic;

namespace NamingServer
{
    public class DataBase
    {
        private SQLiteConnection conn = new SQLiteConnection("Data Source=ns.db;Version=3;");
        //private SQLiteCommand dbQuery;

        public DataBase()
        {
            try
            {
                conn.Open();
                SQLiteCommand dbQuery = conn.CreateCommand();
                ExecuteNonQuery("CREATE TABLE IF NOT EXISTS dirs(curr_path TEXT, name TEXT, parent_path TEXT, file_id INTEGER, PRIMARY KEY(curr_path)); " +
                             "CREATE TABLE IF NOT EXISTS files(id INTEGER, path TEXT, name TEXT, addr TEXT, reserv_addr TEXT, PRIMARY KEY(path, name), FOREIGN KEY(id) REFERENCES dirs(file_id)); " +
                                "CREATE TABLE IF NOT EXISTS storages(id TEXT, ip TEXT, port TEXT, free_space TEXT, PRIMARY KEY(id))");
            }
            catch (SQLiteException err)
            {
                Console.WriteLine(err.Message);
            }
        }

        public void ExecuteNonQuery(string query)
        {
            try
            {
                SQLiteCommand dbQuery = conn.CreateCommand();
                dbQuery.CommandText = query;
                dbQuery.ExecuteNonQuery();
            }
            catch (SQLiteException err)
            {
                Console.WriteLine(err.Message);
            }
        }

        public List<Directory> GetDirsFromDB(string condition)
        {
            List<Directory> dirs = new List<Directory>();
            SQLiteCommand dbQuery = conn.CreateCommand();
            dbQuery.CommandText = "SELECT * from dirs WHERE "+condition;
            SQLiteDataReader reader = dbQuery.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    dirs.Add(new Directory(reader["name"].ToString(), reader["parent_path"].ToString(), reader["curr_path"].ToString()));
                }
                reader.Close();
            }
            catch (SQLiteException err)
            {
                Console.WriteLine(err.Message);
            }
            return dirs;
        }

        public bool StorageCheck(string ip, string port)
        {
            SQLiteCommand dbQuery = conn.CreateCommand();
            dbQuery.CommandText = "SELECT * from storages WHERE ip='" + ip + "' AND port='" + port + "'";
            SQLiteDataReader reader = dbQuery.ExecuteReader();
            if (reader.Read())
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        public void CloseConnection()
        {
            conn.Dispose();
        }
    }
}
