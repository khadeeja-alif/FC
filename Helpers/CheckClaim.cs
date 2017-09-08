using FC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FC.Helpers
{
    public static class CheckClaim
    {
        public static string GetToken(User user, IConfigurationRoot config)
        {
            // var user = HttpContext.User;
            var claims = new List<Claim>
             {
                 new Claim("id",user.id.ToString())
             };
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["jwtSecretKey"]));

            var jwt = new JwtSecurityToken(
               issuer: config["issuer"],
               audience: config["audience"],
               claims: claims,
               notBefore: DateTime.UtcNow,
               expires: DateTime.UtcNow.Add(TimeSpan.FromDays(10)),
               signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        public static string GetApiToken(IConfigurationRoot config)
        {
            var claims = new List<Claim>
             {
                 new Claim("API_KEY",config["API_KEY"])
             };
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["jwtSecretKey"]));

            var jwt = new JwtSecurityToken(
               issuer: config["issuer"],
               audience: config["audience"],
               claims: claims,
               //notBefore: DateTime.UtcNow,
              // expires: DateTime.UtcNow.Add(TimeSpan.FromDays(365)),
               signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

    }
}
