using System;
using Nancy;

namespace NamingServer
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            //CORS Enable
            pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
            {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                                .WithHeader("Access-Control-Allow-Methods", "POST,GET")
                                .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");

            });
        }
    }

    public class ClientAPI : NancyModule
    {
        public ClientAPI() 
        {
            Get["/list"] = param =>
            {
                try
                {
                    Directory directory = FileSystem.GetDirectory(Request.Query["path"]);
                    return JsonResponses.GetDirectoryJson(directory);
                }
                catch (Exception err)
                {
                    Console.WriteLine("list: "+err.Message);
                    return 404;
                }
            };
            Get["/createDir"] = param =>
            {
                try
                {
                    Directory penDir = FileSystem.GetDirectory(Request.Query["path"]);
                    penDir.CreateSubDir(Request.Query["name"]);
                    Console.WriteLine("Created dir "+Request.Query["path"] + Request.Query["name"]);
                    return 200;
                }
                catch (Exception err)
                {
                    Console.WriteLine("createDir: " + err.Message);
                    return 500;
                }
            };
            Get["/delDir"] = param =>
            {
                try
                {
                    Directory penDir = FileSystem.GetDirectory(Request.Query["path"]);
                    penDir.DeleteSubDir(Request.Query["name"]);
                    return 200;
                }
                catch (Exception err)
                {
                    Console.WriteLine("delDir: "+err.Message);
                    return 404;
                }
            };
            Get["/uploadFile"] = param =>
            {
                try
                {   
                    return JsonResponses.GetStorageToUpload(FileSystem.db.ChooseStorage(Request.Query["size"]));
                }
                catch (Exception err)
                {
                    Console.WriteLine("uploadFile: "+err.Message);
                    return 500;
                }
            };
        }
    }

    public class StorageAPI : NancyModule
    {
        public StorageAPI()
        {
            Get["/storageReg"] = param =>
            {
                try
                {
                    if (!FileSystem.db.StorageCheck(Request.UserHostAddress, Request.Query["port"]))
                    {
                        FileSystem.db.ExecuteNonQuery("INSERT INTO storages(id, ip, port, free_space) VALUES('" + Request.Query["id"] + "', '" + Request.UserHostAddress + "', '" + Request.Query["port"] + "', '" + Request.Query["free_space"] + "');");
                        Console.WriteLine("Registered storage server: " + Request.UserHostAddress);
                        return 200;
                    }
                    else
                    {
                        return 500;
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine("storageReg: " + err.Message);
                    return 409;
                }
            };
            Get["/storageConn"] = param =>
            {
                try
                {
                    FileSystem.db.ExecuteNonQuery("UPDATE storages SET ip='" + Request.UserHostAddress + "', port='" + Request.Query["port"] + "', free_space='" + Request.Query["free_space"] + "' WHERE id='" + Request.Query["id"] + "'");
                    return 200;
                }
                catch (Exception err)
                {
                    Console.WriteLine("storageConn: " + err.Message);
                    return 500;
                }
            };
            Get["/fileReg"] = param =>
            {
                try
                {
                    Directory dir = FileSystem.GetDirectory(Request.Query["path"]);
                    dir.RegFile(Request.Query["name"], FileSystem.db.UpdateStorageFreeSpace(Request.Query["id"], Request.Query["free_space"]), Request.Query["id"]);
                    return 200;
                }
                catch
                {
                    return 500;
                }
            };
        }
    }
}
