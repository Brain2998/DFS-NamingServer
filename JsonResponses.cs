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
            dirJson.currentPath = directory.CurrentPath;
            dirJson.items = new JArray();
            foreach (KeyValuePair<string, Directory>subdir in directory.Directories)
            {
                dynamic item = new JObject();
                item.name = subdir.Key;
                item.type = "dir";
                dirJson.items.Add(item);
            }
            foreach (KeyValuePair<string, DirFile> file in directory.Files)
            {
                dynamic item = new JObject();
                item.name = file.Key;
                item.type = "file";
                item.address = file.Value.Address;
                item.reserveAddress = file.Value.ReserveAddress;
                dirJson.items.Add(item);
            }
            return dirJson.ToString();
        }
    }
}
