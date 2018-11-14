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
                ExecuteNonQuery("CREATE TABLE IF NOT EXISTS dirs(curr_path TEXT, name TEXT, parent_path TEXT, PRIMARY KEY(curr_path)); " +
                             "CREATE TABLE IF NOT EXISTS files(full_path TEXT, dir_path TEXT, name TEXT, addr TEXT, reserv_addr TEXT, size TEXT, PRIMARY KEY(full_path)); " +
                                "CREATE TABLE IF NOT EXISTS storages(id TEXT, ip TEXT, port TEXT, free_space TEXT, PRIMARY KEY(id));" +
                                "CREATE TABLE IF NOT EXISTS file_to_dir(dir_path TEXT, file_path TEXT, PRIMARY KEY(dir_path, file_path));");
            }
            catch (SQLiteException err)
            {
                Console.WriteLine("DB initialization: "+err.Message);
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
                Console.WriteLine("ExecuteNonQuery: "+err.Message);
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
                Console.WriteLine("GetDirsFromDB: "+err.Message);
            }
            return dirs;
        }

        public bool StorageCheck(string ip, string port)
        {
            SQLiteCommand dbQuery = conn.CreateCommand();
            dbQuery.CommandText = "SELECT * from storages WHERE ip='" + ip + "' AND port='" + port + "'";
            SQLiteDataReader reader = dbQuery.ExecuteReader();
            bool RC;
            if (reader.Read())
            {
                RC=true;
            }
            else 
            {
                RC=false;
            }
            reader.Close();
            return RC;
        }

        public string ChooseStorage(string reqSpace)
        {
            string storage="";
            SQLiteCommand dbQuery = conn.CreateCommand();
            dbQuery.CommandText = "SELECT ip, port FROM storages WHERE free_space +0 > "+reqSpace+" ORDER BY free_space +0 DESC";
            SQLiteDataReader reader = dbQuery.ExecuteReader();
            try
            {
                if (reader.Read())
                {
                    storage= reader["ip"].ToString() + ":" + reader["port"].ToString();
                }
                else
                {
                    throw new Exception("No storage servers available.");
                }
            }
            catch (SQLiteException err)
            {
                Console.WriteLine("ChooseStorage: " + err.Message);
            }
            reader.Close();
            return storage;
        }

        public string UpdateStorageFreeSpace(string id, string newSpace)
        {
            Int64 oldSpaceInt=0;
            Int64 newSpaceInt = 0;
            Int64.TryParse(newSpace, out newSpaceInt);
            SQLiteCommand dbQuery = conn.CreateCommand();
            dbQuery.CommandText = "SELECT free_space FROM storages WHERE id='" + id + "'";
            SQLiteDataReader reader = dbQuery.ExecuteReader();
            if (reader.Read())
            {
                //Console.WriteLine(reader["free_space"]);
                Int64.TryParse(reader["free_space"].ToString(), out oldSpaceInt);
            }
            reader.Close();
            FileSystem.db.ExecuteNonQuery("UPDATE storages SET free_space='" + newSpace + "' WHERE id='" + id + "'");
            return (Math.Abs(oldSpaceInt - newSpaceInt)).ToString();
        }

        public string GetStorageAddressById(string id)
        {
            string storageAddr = "";
            SQLiteCommand dbQuery = conn.CreateCommand();
            dbQuery.CommandText = "SELECT ip, port FROM storages WHERE id='" + id + "'";
            SQLiteDataReader reader = dbQuery.ExecuteReader();
            try
            {
                if (reader.Read())
                {
                    storageAddr = reader["ip"].ToString() + ":" + reader["port"].ToString();
                }
                reader.Close();
            }
            catch (SQLiteException err)
            {
                Console.WriteLine("GetStorageById: " + err.Message);
            }
            return storageAddr;
        }

        public List<DirFile> GetFilesFromDB(string condition)
        {
            List<DirFile> files = new List<DirFile>();
            SQLiteCommand dbQuery = conn.CreateCommand();
            dbQuery.CommandText = "SELECT * from files WHERE " + condition;
            SQLiteDataReader reader = dbQuery.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    files.Add(new DirFile(reader["name"].ToString(), reader["addr"].ToString(), reader["size"].ToString()));
                }
                reader.Close();
            }
            catch (SQLiteException err)
            {
                Console.WriteLine("GetFilesFromDB: " + err.Message);
            }
            return files;
        }

        public void CloseConnection()
        {
            conn.Dispose();
        }
    }
}
