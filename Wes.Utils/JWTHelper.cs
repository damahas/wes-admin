using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Wes.Utils
{
    public static class JWTHelper
    {
        public static string GenerateToken(string userId)
        {
            var claims = new[]
            {
             //JwtRegisteredClaimNames.Sub:
             // 这是一个预定义的 JWT 声明类型，表示 "subject"（主体）。
             // 在此上下文中，它通常用于存储用户的唯一标识符（如用户名或用户ID）。
             new Claim("userId", userId),
            };
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: GlobalContext.JwtSettings.Issuer,
                audience: GlobalContext.JwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(GlobalContext.JwtSettings.Expires),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GlobalContext.JwtSettings.SecretKey)), SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GetUserId(HttpContext httpContext)
        {
            string userId = "";
            if (httpContext.User == null || !httpContext.User.Claims.Any())
                return userId;
            var claim = httpContext.User.Claims.FirstOrDefault(e => e.Type == "userId");
            if (claim == null)
                return userId;
            userId = claim.Value;
            return userId;
        }

        public static string GetToken(string str) {
            if (string.IsNullOrWhiteSpace(str)) {
                return "";
            }
            return str.Replace("Bearer ", "");
        }
    }
}
