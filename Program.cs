using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using FC.Filters;
using Microsoft.AspNetCore.Http;

namespace FC
{
    public class Program
    {
       // [ApiAuthenticator("API_KEY")]
        public static void Main(string[] args)
        {

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseSetting("detailedErrors", "true")
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .CaptureStartupErrors(true)
                .Build();
            host.Run();
        }
    }
}
