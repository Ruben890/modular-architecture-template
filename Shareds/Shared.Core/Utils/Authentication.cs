using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.DTO.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Core.Utils
{

    public static class Authentication
    {
        private static readonly string issuer;
        private static readonly string audience;
        private static readonly string key;

        static Authentication()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .Build();

            issuer = GetConfigurationValue(configuration, "Jwt:Issuer");
            audience = GetConfigurationValue(configuration, "Jwt:Audience");
            key = GetConfigurationValue(configuration, "Jwt:Key");
        }

        private static string GetConfigurationValue(IConfiguration configuration, string key)
        {
            return configuration[key] ?? throw new InvalidOperationException($"The value '{key}' in the configuration cannot be null or empty.");
        }

        private static IEnumerable<Claim> GenerateClaims(object data)
        {
            var claims = new List<Claim>();

            if (data != null)
            {
                if (data.GetType().IsValueType || data is string)
                {
                    claims.Add(new Claim("Value", data.ToString()!));
                }
                else
                {
                    var properties = data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var property in properties)
                    {
                        var value = property.GetValue(data)?.ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            claims.Add(new Claim(property.Name, value));
                        }
                    }
                }
            }

            return claims;
        }

        public static Token GenerateToken(object data, DateTime expires)
        {
            var claims = GenerateClaims(data);
            var accessToken = GenerateJwtToken(claims, expires);
            var refreshToken = GenerateRefreshToken();
            return new Token(accessToken, refreshToken);
        }

        private static string GenerateJwtToken(IEnumerable<Claim> claims, DateTime expires)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
