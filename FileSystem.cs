using System;
using System.Collections.Generic;

namespace NamingServer
{
    public static class FileSystem
    {
        public static Directory root=new Directory("/", "/", "/");
        public static DataBase db = new DataBase();

        public static void FillDirsFromDB(Directory startDir)
        {
            Directory currDir = startDir;
            List<Directory> Dirs = db.GetDirsFromDB("parent_path='" + currDir.CurrentPath+"'");
            List<DirFile> Files = db.GetFilesFromDB("full_path IN (SELECT file_path FROM file_to_dir WHERE dir_path='"+currDir.CurrentPath+"')");
            for (int i = 0; i < Dirs.Count; ++i)
            {
                currDir.Directories.Add(Dirs[i].Name, Dirs[i]);
                FillDirsFromDB(Dirs[i]);
            }
            for (int i = 0; i < Files.Count; ++i)
            {
                currDir.Files.Add(Files[i].Name, Files[i]);
            }
        }

        public static Directory GetDirectory(string path)
        {
            if (path[0]!='/')
            {
                throw new Exception("No such directory");
            }
            if (path == "/")
            {
                return root;
            }
            else
            {
                string[] dirs = path.Split('/');
                Directory endDir = root;
                for (int i = 1; i < dirs.Length; ++i)
                {
                    if (endDir.Directories.ContainsKey(dirs[i]))
                    {
                        endDir = endDir.Directories[dirs[i]];
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                return endDir;
            }
        }
    }

    public class Directory
    {
        public string Name { get; set; }
        public string ParentPath { get; set; }
        public string CurrentPath { get; set; }
        public Dictionary<string, Directory> Directories = new Dictionary<string, Directory>();
        public Dictionary<string, DirFile> Files = new Dictionary<string, DirFile>();

        public Directory(string name, string parentPath, string currentPath)
        {
            Name = name;
            ParentPath = parentPath;
            CurrentPath = currentPath;
        }

        public void CreateSubDir(string isertedName)
        {
            if (Directories.ContainsKey(isertedName))
            {
                throw new Exception("Directory already exist");
            }
            if (isertedName=="" || isertedName=="/")
            {
                throw new Exception("Can not create directory with specified name");
            }
            Directory insertedDir;
            if (Name == "/")
            {
                insertedDir = new Directory(isertedName, Name, Name+isertedName);
                Directories.Add(isertedName, insertedDir);
            }
            else
            {
                if (ParentPath == "/")
                {
                    insertedDir = new Directory(isertedName, ParentPath+Name, ParentPath+Name+"/"+isertedName);
                    Directories.Add(isertedName, insertedDir);
                }
                else
                {
                    insertedDir = new Directory(isertedName, ParentPath + "/" + Name, ParentPath + "/" + Name+"/"+isertedName);
                    Directories.Add(isertedName, insertedDir);
                }
            }
            FileSystem.db.ExecuteNonQuery("INSERT INTO dirs(curr_path, name, parent_path) VALUES ('"+insertedDir.CurrentPath+"', '"+insertedDir.Name+"', '"+insertedDir.ParentPath+"')");
        }

        public void DeleteSubDir(string deletedName)
        {
            if (!Directories.ContainsKey(deletedName))
            {
                throw new Exception("No such directory");
            }
            Directory deletedDir = Directories[deletedName];
            FileSystem.db.ExecuteNonQuery("DELETE FROM dirs WHERE curr_path='" + deletedDir.CurrentPath + "')");
            Directories.Remove(deletedName);
        }

        public void RegFile(string name, string size, string storageId)
        {
            string mainAddress = FileSystem.db.GetStorageAddressById(storageId);
            string filePath;
            if (CurrentPath == "/")
            {
                filePath = CurrentPath + name;
            }
            else
            {
                filePath = CurrentPath + "/" + name;
            }
            Files.Add(name, new DirFile(name, mainAddress, size));
            //select reserve server
            FileSystem.db.ExecuteNonQuery("INSERT INTO files(full_path, dir_path, name, addr, size) VALUES ('" + filePath + "', '" + CurrentPath + "', '"+name+"', '" + mainAddress + "', '" + size + "')");
            FileSystem.db.ExecuteNonQuery("INSERT INTO file_to_dir(dir_path, file_path) VALUES ('" + CurrentPath + "', '" + filePath + "')");
        }
    }

    public class DirFile
    {
        public DirFile(string name, string address, string size)
        {
            Name = name;
            Address = address;
            Size = size;
        }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ReserveAddress { get; set; }
        public string Size { get; set; }
    }
}
