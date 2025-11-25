using System;
using System.Security.Cryptography;
using System.Text;

namespace Payroll_Web_App.Server.Security
{
    // <summary>
    // Password hashing with PBKDF2 (SHA-256).
    // Stored format: PH1|{iterations}|{saltBase64}|{hashBase64}
    
    public static class PasswordHasher
    {
        private const int Iterations = 210_000;
        private const int SaltSizeBytes = 16;
        private const int HashSizeBytes = 32;
        private const string Prefix = "PH1";

        public static string HashPassword(string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            Span<byte> salt = stackalloc byte[SaltSizeBytes];
            RandomNumberGenerator.Fill(salt);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt.ToArray(),
                iterations: Iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: HashSizeBytes
            );

            return $"{Prefix}|{Iterations}|{Convert.ToBase64String(salt)}|{Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string stored)
        {
            if (password == null || string.IsNullOrWhiteSpace(stored))
                return false;

            var parts = stored.Split('|');
            if (parts.Length != 4 || parts[0] != Prefix)
                return false;

            int iterations = int.Parse(parts[1]);
            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] expectedHash = Convert.FromBase64String(parts[3]);

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt,
                iterations: iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: expectedHash.Length
            );

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
