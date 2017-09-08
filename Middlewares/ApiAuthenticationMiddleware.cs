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
        private IConfigurationRoot _config;
        private TokenValidationParameters validationParameters;
        public ApiAuthenticationMiddleware(RequestDelegate next, IConfigurationRoot config)
        {
            _next = next;
            _config = config;
            var key = _config["jwtSecretKey"];
            var issuer = _config["issuer"];
            var audience = _config["audience"];
            validationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = issuer,


                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = audience,

                // Validate the token expiry
                ValidateLifetime = false,

                // If you want to allow a certain amount of clock drift, set that here:
                //ClockSkew = TimeSpan.Zero
            };
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("API_KEY"))
            {
                string ApiKey = context.Request.Headers["API_KEY"];
                var handler = new JwtSecurityTokenHandler();
                ClaimsPrincipal principal = null;
                SecurityToken validToken = null;
                try
                {
                    if (ApiKey.StartsWith("ApiKey ", StringComparison.OrdinalIgnoreCase))
                    {
                        ApiKey = ApiKey.Substring("ApiKey ".Length).Trim();
                    }

                    principal = handler.ValidateToken(ApiKey, this.validationParameters, out validToken);

                    var validJwt = validToken as JwtSecurityToken;

                    if (validJwt == null)
                    {
                        throw new ArgumentException("Invalid JWT");
                    }

                    //ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(validJwt.Claims));
                    context.User = principal;

                    return this._next(context);

                }
                catch (SecurityTokenValidationException)
                {
                    return this._next(context);
                }
                catch (ArgumentException)
                {
                    return this._next(context);
                }
            }
            return this._next(context);
        }
    }
}