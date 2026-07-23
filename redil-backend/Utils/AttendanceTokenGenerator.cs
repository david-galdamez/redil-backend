using System.Security.Cryptography;

namespace redil_backend.Utils
{
    public static class AttendanceTokenGenerator
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string Generate()
        {
            var bytes = RandomNumberGenerator.GetBytes(8);
            var chars = new char[8];

            for(int i = 0; i < chars.Length; i++)
            {
                var index = bytes[i] % Alphabet.Length;
                chars[i] = Alphabet[index];
            }

            return $"AST-{new string(chars)}";
        }
    }
}
