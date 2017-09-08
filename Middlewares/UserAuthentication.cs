using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Middlewares
{
    public static class UserAuthenticationMiddlewareExtension
    {
        public static IApplicationBuilder UseUserAuthenticationExtension(this IApplicationBuilder app)
        {
            return app.UseMiddleware<UserAuthenticationMiddleware>();
        }
    }    
}
