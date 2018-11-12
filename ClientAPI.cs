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
                    Directory directory = FileSystem.GetDirectory(Request.Query["name"]);
                    return JsonResponses.GetDirectoryJson(directory);
                }
                catch
                {
                    return 404;
                }
            };
            Get["/createDir"] = param =>
            {
                try
                {
                    Directory penDir = FileSystem.GetDirectory(Request.Query["path"]);
                    penDir.CreateSubDir(Request.Query["name"]);
                    return 200;
                }
                catch
                {
                    return 400;
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
                catch
                {
                    return 400;
                }
            };
        }
    }
}
