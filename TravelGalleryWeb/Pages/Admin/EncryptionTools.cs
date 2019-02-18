using System;
using System.Security.Cryptography;
using System.Text;

namespace TravelGalleryWeb.Pages.Admin.Admins
{
    internal static class EncryptionTools
    {
        private const string Salt = "c0efb10d15a14af2a6b07ce5208cc7d8";//"3769011ffb974839a44d2110f9683bf7";
        
        internal static string HashPassword(string password)
        {
            var md5Hasher = new MD5CryptoServiceProvider();
            var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(password+Salt));
            var sBuilder = new StringBuilder();
            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }
            return sBuilder.ToString();
        }
        
        internal static bool VerifyPassword(string input, string hash)
        {
            var hashOfInput = HashPassword(input);
            var comparer = StringComparer.OrdinalIgnoreCase;
            return 0 == comparer.Compare(hashOfInput, hash);
        }
    }
}