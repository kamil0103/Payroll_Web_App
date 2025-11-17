using System;
using System.Security.Cryptography;
using System.Text;

//namespace.Hashing
namespace Payroll.Hashing
{
    
    /// Password hashing with PBKDF2 (SHA-256) and versioned format.
    /// Stored format: PH1|{iterations}|{saltBase64}|{hashBase64}
   
    public static class PasswordHasher
    {
        //  Higher = stronger but slower.
        // 210k iterations aligns with current guidance for SHA-256 PBKDF2 on modern hardware.
        private const int Iterations = 210_000;
        private const int SaltSizeBytes = 16;   // 128-bit salt
        private const int HashSizeBytes = 32;   // 256-bit hash
        private const string Prefix = "PH1";    

       
        /// Hash a plaintext password. Returns a string safe to store in Database.
       
        public static string HashPassword(string password)
        {
            if (password is null) throw new ArgumentNullException(nameof(password));

            // Generate cryptographically strong salt
            Span<byte> salt = stackalloc byte[SaltSizeBytes];
            RandomNumberGenerator.Fill(salt);

            // Derive key using PBKDF2
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt.ToArray(),
                iterations: Iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: HashSizeBytes);

            string saltB64 = Convert.ToBase64String(salt);
            string hashB64 = Convert.ToBase64String(hash);

            // Format: version|iterations|salt|hash
            return $"{Prefix}|{Iterations}|{saltB64}|{hashB64}";
        }

        
        /// Verify a plaintext password against a stored hash string.
        
        public static bool Verify(string password, string stored)
        {
            if (password is null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(stored)) return false;

            var parts = stored.Split('|', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4 || parts[0] != Prefix)
                return false; // Error

            if (!int.TryParse(parts[1], out int iterations) || iterations < 1)
                return false;

            byte[] salt, expHash;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expHash = Convert.FromBase64String(parts[3]);
            }
            catch
            {
                return false; // invalid
            }

            // Recompute hash with same parameters
            byte[] actHash = Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt,
                iterations: iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: expHash.Length);

            // Constant-time comparison for prevent timing attacks
            return CryptographicOperations.FixedTiingmeEquals(actHash, expHash);
        }

      
    }
}
