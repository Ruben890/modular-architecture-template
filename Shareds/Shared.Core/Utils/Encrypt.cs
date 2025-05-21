using Konscious.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Core.Utils
{
    public static class Encrypt
    {
        private static readonly string key;

        // Constructor estático para inicializar la clave
        static Encrypt()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .Build();

            key = GetConfigurationValue(configuration, "Key");
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
            return configuration[key] ?? throw new InvalidOperationException($"The value'{key}' in the configuration cannot be null or empty.");
        }


        // Método para hashear una contraseña con Argon2
        public static string HashPassword(string password)
        {
            // Convierte la contraseña a bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Genera un salt aleatorio de manera eficiente
            byte[] salt = GenerateSalt(16); // Salt de 16 bytes

            // Configuración de Argon2id
            var argon2 = new Argon2id(passwordBytes)
            {
                Salt = salt,
                DegreeOfParallelism = 4,
                MemorySize = 1024 * 64,  // 64MB de memoria (ajustable según necesidad)
                Iterations = 3           // Iteraciones moderadas para buen rendimiento y seguridad
            };

            // Genera el hash de 32 bytes
            byte[] hash = argon2.GetBytes(32);

            // Combina el salt y el hash para almacenarlo
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        // Método para verificar la contraseña
        public static bool VerifyPassword(string password, string storedHash)
        {
            // Separa el salt y el hash guardado
            var parts = storedHash.Split('.');
            if (parts.Length != 2)
                return false;

            // Convierte los componentes a bytes
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

            // Convierte la contraseña proporcionada a bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Configura Argon2id con el salt extraído
            var argon2 = new Argon2id(passwordBytes)
            {
                Salt = salt,
                DegreeOfParallelism = 4,
                MemorySize = 1024 * 64,  // 64MB de memoria
                Iterations = 3           // Misma cantidad de iteraciones que en el hasheado
            };

            // Genera el hash para comparar
            byte[] hashToCompare = argon2.GetBytes(32);

            // Compara los hashes de manera constante (segura contra ataques de tiempo)
            return CryptographicOperations.FixedTimeEquals(hashToCompare, storedHashBytes);
        }

        // Método optimizado para generar un salt seguro
        private static byte[] GenerateSalt(int size)
        {
            var salt = new byte[size];
            // Usa RandomNumberGenerator en lugar de RNGCryptoServiceProvider
            RandomNumberGenerator.Fill(salt);
            return salt;
        }

        // Método para cifrar texto
        public static string EncryptString(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                // Asegúrate de que la longitud de la clave sea de 32 bytes (256 bits)
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                if (keyBytes.Length != 32) // 256 bits para AES
                {
                    throw new ArgumentException("The key length must be 32 bytes (256 bits).");
                }

                aes.Key = keyBytes;
                aes.GenerateIV(); // Generar un nuevo IV

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    // Escribir el IV al principio del flujo
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string DecryptString(string cipherText)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                // Asegúrate de que la longitud de la clave sea de 32 bytes (256 bits)
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                if (keyBytes.Length != 32) // 256 bits para AES
                {
                    throw new ArgumentException("The key length must be 32 bytes (256 bits).");
                }

                aes.Key = keyBytes;

                // Extraer el IV del flujo cifrado
                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static string Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input), "The input value cannot be null or empty.");

            using (var sha256 = SHA256.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha256.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

    }
}
