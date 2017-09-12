using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace FC.Middlewares
{
    public class ApiAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private string  _key;
        //private TokenValidationParameters validationParameters;
        public ApiAuthenticationMiddleware(RequestDelegate next, IConfigurationRoot config)
        {
            _next = next;
            _key = config["API-KEY"];
           
          }

        public Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("API-KEY"))
            {
                string key = context.Request.Headers["API-KEY"];
                try
                {
                    if (key != _key)
                    {
                        context.Response.StatusCode = 401;
                        return context.Response.WriteAsync("Authentication failed");
                    }
                    else
                    {
                        return _next(context);
                    }


                }
                catch (Exception)
                {
                    context.Response.StatusCode = 401;
                    return context.Response.WriteAsync("Authentication failed");
                }
            }
            else
            {
                context.Response.StatusCode = 401;
                return context.Response.WriteAsync("Authentication failed");
            }
        }
    }
}