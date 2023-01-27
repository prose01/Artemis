using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Artemis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    // TODO: Look at Kestrel https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-7.0#kestrel-maximum-request-body-size
                    //webBuilder.ConfigureKestrel((context, options) =>
                    //{
                    //    // Handle requests up to 50 MB
                    //    options.Limits.MaxRequestBodySize = 52428800;
                    //})
                    //.UseStartup<Startup>();
                });
    }
}
