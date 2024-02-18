using System.Security.Cryptography;

namespace aspnetapp.Auth
{
    public static class CookieCreator
    {
        public const int AcountLength = 40;

        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

        private static readonly char[] characters = new char[] {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        public static string Crate(int length)
        {
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);

            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = characters[bytes[i] % characters.Length];

            return new string(chars);
        }
    }
}