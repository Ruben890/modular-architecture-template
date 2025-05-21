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
    /// <summary>
    /// Clase estática que proporciona utilidades para la autenticación y generación de tokens JWT.
    /// </summary>
    public static class Authentication
    {
        private static readonly string issuer;
        private static readonly string audience;
        private static readonly string key;

        /// <summary>
        /// Constructor estático para inicializar los valores de configuración.
        /// </summary>
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

        /// <summary>
        /// Obtiene el valor de una clave de configuración y lanza una excepción si el valor es nulo o vacío.
        /// </summary>
        /// <param name="configuration">La configuración de la cual obtener el valor.</param>
        /// <param name="key">La clave de configuración.</param>
        /// <returns>El valor de configuración.</returns>
        /// <exception cref="InvalidOperationException">Se lanza si el valor de configuración es nulo o vacío.</exception>
        private static string GetConfigurationValue(IConfiguration configuration, string key)
        {
            return configuration[key] ?? throw new InvalidOperationException($"El valor '{key}' en la configuración no puede ser nulo o vacío.");
        }

        /// <summary>
        /// Genera una lista de <see cref="Claim"/> a partir de las propiedades públicas de un objeto.
        /// </summary>
        /// <param name="obj">El objeto del cual generar los claims.</param>
        /// <returns>Una lista de claims.</returns>
        private static IEnumerable<Claim> GenerateClaims(object data)
        {
            var claims = new List<Claim>();

            if (data != null)
            {
                // Si es una sola propiedad, agregamos una sola claim
                if (data.GetType().IsValueType || data.GetType() == typeof(string))
                {
                    claims.Add(new Claim(data.GetType().Name, data.ToString()!));
                }
                else // Si es un objeto completo, agregamos todas las propiedades como claims
                {
                    var properties = data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var property in properties)
                    {
                        var value = property.GetValue(data)?.ToString();
                        if (value != null)
                        {
                            claims.Add(new Claim(property.Name, value));
                        }
                    }
                }
            }

            return claims;
        }

        /// <summary>
        /// Genera un token JWT basado en los claims generados a partir de un objeto y una fecha de expiración.
        /// </summary>
        /// <param name="data">El objeto del cual generar los claims.</param>
        /// <param name="expires">La fecha y hora de expiración del token.</param>
        /// <returns>El token JWT como una cadena.</returns>
        public static Token GenerateToken(object data, DateTime expires)
        {
            var claims = GenerateClaims(data);
            var token = GenerateToken(claims, expires);

            var refreshToken = GenerateRefreshToken();

            return new Token(token, refreshToken);
        }

        /// <summary>
        /// Generates a secure random refresh token.
        /// </summary>
        /// <returns>A base64 encoded string representing the refresh token.</returns>
        /// <remarks>
        /// This method creates a secure random token by generating 32 bytes of random data 
        /// and then encoding it using Base64. The RandomNumberGenerator class is used to 
        /// ensure cryptographic strength of the random numbers.
        /// </remarks>
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// Genera un token JWT basado en una lista de claims y una fecha de expiración.
        /// </summary>
        /// <param name="claims">La lista de claims para incluir en el token.</param>
        /// <param name="expires">La fecha y hora de expiración del token.</param>
        /// <returns>El token JWT como una cadena.</returns>
        private static string GenerateToken(IEnumerable<Claim> claims, DateTime expires)
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
    }
}
