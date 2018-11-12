using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace NamingServer
{
    public static class JsonResponses
    {
        public static string GetDirectoryJson(Directory directory)
        {
            dynamic dirJson = new JObject();
            dirJson.name = directory.Name;
            dirJson.parentPath = directory.ParentPath;
            if (directory.Name == "/")
            {
                dirJson.currentPath = "/";
            }
            else
            {
                if (directory.ParentPath == "/")
                {
                    dirJson.currentPath = directory.ParentPath + directory.Name;
                }
                else
                {
                    dirJson.currentPath = directory.ParentPath + "/" + directory.Name;
                }
            }
            dirJson.items = new JArray();
            foreach (KeyValuePair<string, Directory>subdir in directory.Directories)
            {
                dynamic item = new JObject();
                item.name = subdir.Key;
                item.type = "dir";
                dirJson.items.Add(item);
            }
            foreach (KeyValuePair<string, File> file in directory.Files)
            {
                dynamic item = new JObject();
                item.name = file.Key;
                item.type = "file";
                item.address = file.Value.Address;
                dirJson.items.Add(item);
            }
            return dirJson.ToString();
        }
    }
}
