using System.Security.Cryptography;
using System.Text;

namespace aspnetapp
{
    public static class Utils
    {
        private static SHA256 sha256 = SHA256.Create();

        public static Guid GetHash(this string s)
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));
            bytes = bytes.Take(16).ToArray(); // take first 16 bytes (hash returns 32)
            return new Guid(bytes);
        }
    }
}
