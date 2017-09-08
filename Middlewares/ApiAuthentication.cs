using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Middlewares
{
    public static class ApiAuthenticationMiddlewareExtension
    {
        public static IApplicationBuilder UseApiAuthenticationExtension(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ApiAuthenticationMiddleware>();
        }
    }
}
