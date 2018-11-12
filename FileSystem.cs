using System;
using System.Collections.Generic;

namespace NamingServer
{
    public static class FileSystem
    {
        public static Directory root=new Directory("/", "/");

        public static Directory GetDirectory(string path)
        {
            if (path[0]!='/')
            {
                throw new Exception();
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
        public Directory(string name, string parentPath)
        {
            Name = name;
            ParentPath = parentPath;
        }
        public string Name { get; set; }
        public string ParentPath { get; set; }
        public Dictionary<string, Directory> Directories = new Dictionary<string, Directory>();
        public Dictionary<string, File> Files = new Dictionary<string, File>();

        public void CreateSubDir(string name)
        {
            if (Directories.ContainsKey(name))
            {
                throw new Exception();
            }
            if (Name == "/")
            {
                Directories.Add(name, new Directory(name, "/"));
            }
            else
            {
                if (ParentPath == "/")
                {
                    Directories.Add(name, new Directory(name, ParentPath + Name));
                }
                else
                {
                    Directories.Add(name, new Directory(name, ParentPath + "/" + Name));
                }
            }
        }

        public void DeleteSubDir(string name)
        {
            if (!Directories.ContainsKey(name))
            {
                throw new Exception();
            }
            Directories.Remove(name);
        }
    }

    public class File
    {
        public File(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
