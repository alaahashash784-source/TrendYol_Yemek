// مساعد الأمان - تشفير كلمات المرور بـ PBKDF2
using System;
using System.Security.Cryptography;
using System.Text;

namespace mvc_full.Helpers
{
    /// <summary>
    /// Security helper class for password hashing and verification.
    /// Uses PBKDF2 with SHA256 for secure password hashing.
    /// </summary>
    public static class SecurityHelper
    {
        // Constants for password hashing
        private const int SaltSize = 16; // 128 bits
        private const int HashSize = 32; // 256 bits
        private const int Iterations = 10000; // PBKDF2 iterations

        /// <summary>
        /// Creates a hash from a password using PBKDF2 with a random salt.
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns>Base64 encoded string containing salt and hash</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            // Generate a random salt
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Create the hash
            byte[] hash = CreateHash(password, salt);

            // Combine salt and hash
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Return as base64 string
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Verifies a password against a stored hash.
        /// </summary>
        /// <param name="password">The password to verify</param>
        /// <param name="storedHash">The stored hash to verify against</param>
        /// <returns>True if the password matches, false otherwise</returns>
        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            {
                return false;
            }

            try
            {
                // Get the bytes from the stored hash
                byte[] hashBytes = Convert.FromBase64String(storedHash);

                // Check if it's a valid hash (has salt + hash)
                if (hashBytes.Length != SaltSize + HashSize)
                {
                    // Might be a legacy plain text password or simple hash
                    // For backward compatibility, try simple comparison
                    return storedHash == password || 
                           storedHash == Convert.ToBase64String(Encoding.UTF8.GetBytes(password + "salt"));
                }

                // Extract salt
                byte[] salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                // Extract stored hash
                byte[] storedPasswordHash = new byte[HashSize];
                Array.Copy(hashBytes, SaltSize, storedPasswordHash, 0, HashSize);

                // Compute hash of the provided password
                byte[] computedHash = CreateHash(password, salt);

                // Compare hashes
                return SlowEquals(storedPasswordHash, computedHash);
            }
            catch
            {
                // If there's any error (invalid base64, etc.), try plain comparison
                // This handles legacy passwords stored in plain text
                return storedHash == password;
            }
        }

        /// <summary>
        /// Creates a PBKDF2 hash from a password and salt.
        /// </summary>
        private static byte[] CreateHash(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        /// <summary>
        /// Compares two byte arrays in constant time to prevent timing attacks.
        /// </summary>
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        /// <summary>
        /// Generates a cryptographically secure random token.
        /// </summary>
        /// <param name="length">Length of the token in bytes</param>
        /// <returns>Base64 encoded token</returns>
        public static string GenerateSecureToken(int length = 32)
        {
            byte[] tokenBytes = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(tokenBytes);
            }
            return Convert.ToBase64String(tokenBytes);
        }

        /// <summary>
        /// Simple string sanitization to prevent basic XSS attacks.
        /// Note: Use proper HTML encoding for display in views.
        /// </summary>
        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Remove potentially dangerous characters
            return input
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
        }
    }
}
