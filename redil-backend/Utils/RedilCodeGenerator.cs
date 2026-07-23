using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Security.Cryptography;

namespace redil_backend.Utils
{
    public static class RedilCodeGenerator
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string Generate()
        {
            var bytes = RandomNumberGenerator.GetBytes(8);
            var chars = new char[8];

            for(int i = 0; i < chars.Length; i++)
            {
                chars[i] = Alphabet[bytes[i] % Alphabet.Length];
            }

            return $"RDL-{new string(chars)}";
        }
    }
}
