using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Helper
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _configuration;
        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(UserEntity user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User object is null");
            }

            var jwtSettings = _configuration.GetSection("Jwt");
            if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings["Key"]) ||
                string.IsNullOrWhiteSpace(jwtSettings["Issuer"]) || string.IsNullOrWhiteSpace(jwtSettings["Audience"]))
            {
                throw new Exception("JWT settings are missing or invalid in configuration.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim("userId", user.Id.ToString()),
        new Claim("userName", user.UserName ?? string.Empty),
        new Claim("email", user.Email ?? string.Empty)
    };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string GenerateResetToken(int userId, string email)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Invalid userId or email for reset token generation.");
            }

            var resetSecret = _configuration["Jwt:ResetSecret"];
            if (string.IsNullOrWhiteSpace(resetSecret))
            {
                throw new Exception("JWT ResetSecret is missing in appsettings.json.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(resetSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim("userId", userId.ToString()),
        new Claim("email", email)
    };

            var token = new JwtSecurityToken(
                issuer: "AddressBook",
                audience: "AddressBookUser",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        public int ResetPassword(string token, ResetPasswordReq model)
        {
            var resetSecret = _configuration["Jwt:ResetSecret"];
            if (string.IsNullOrWhiteSpace(resetSecret))
            {
                throw new Exception("JWT ResetSecret is missing in appsettings.json.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(resetSecret));
            var handler = new JwtSecurityTokenHandler();

            var claimsPrincipal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:ResetIssuer"],
                ValidAudience = _configuration["Jwt:ResetAudience"],
                IssuerSigningKey = key
            },
            out SecurityToken validatedToken);

            var userIdClaim = claimsPrincipal.FindFirst("userId")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
            {
                throw new SecurityTokenException("Invalid Token: UserId Claim Missing");
            }

            return int.Parse(userIdClaim);
        }

    }
}
