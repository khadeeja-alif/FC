﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySQL.Data.EntityFrameworkCore.Extensions;
using FC.Models;
using System.Text.RegularExpressions;
using System;
using FC.Middlewares;
using FC.Filters;
using Microsoft.AspNetCore.Http;

namespace FC
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {            
                var builder = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("config.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
                Configuration = builder.Build();
                     
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            //services.AddDbContext<UserContext>(opt => opt.UseInMemoryDatabase());

            services.AddSingleton(Configuration);
            services.AddMvc();
            
#if DEBUG
            var sqlConnectionString = Configuration.GetConnectionString("DataAccessMySqlProvider");
#else
              string envvariable = Environment.GetEnvironmentVariable("MYSQLCONNSTR_localdb");
              string dbhost = "";
              string dbname = "";
              string dbusername = "";
              string dbpassword = "";
              dbhost = Regex.Replace(envvariable, "^.*Data Source=(.+?);.*$", "$1");
              dbname = Regex.Replace(envvariable, "^.*Database=(.+?);.*$", "$1");
              dbusername = Regex.Replace(envvariable, "^.*User Id=(.+?);.*$", "$1");
              dbpassword = Regex.Replace(envvariable, "^.*Password=(.+?)$", "$1");
              var host = dbhost.Split(':');
              var sqlConnectionString = "server="+host[0]+";userid="+dbusername+";password="+dbpassword+";persistsecurityinfo=true;port="+host[1]+";database=markyourday;Sslmode=none";
#endif
            Console.WriteLine(sqlConnectionString);
            System.Diagnostics.Debug.WriteLine(sqlConnectionString);
            services.AddDbContext<UserContext>(options =>
            {
                options.UseMySQL(
                    sqlConnectionString,
                    b => b.MigrationsAssembly("FC")
                );
            });
            services.AddDbContext<AttendanceContext>(options =>
            {
                options.UseMySQL(
                    sqlConnectionString,
                    b => b.MigrationsAssembly("FC")
                );
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseApiAuthenticationExtension();
            app.UseUserAuthenticationExtension();
            //if(env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
            //}
            app.UseMvc();
           /* app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
            */
        }
    }
}
