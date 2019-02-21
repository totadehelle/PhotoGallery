using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace TravelGalleryWeb.Pages.Admin.Admins
{
    internal class EncryptionTools
    {
        private readonly IOptions<Constants> _config;
        private readonly string Salt;
        public EncryptionTools(IOptions<Constants> config)
        {
            _config = config;
            Salt = _config.Value.Salt;
        }
        internal string HashPassword(string password)
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
        
        internal bool VerifyPassword(string input, string hash)
        {
            var hashOfInput = HashPassword(input);
            var comparer = StringComparer.OrdinalIgnoreCase;
            return 0 == comparer.Compare(hashOfInput, hash);
        }
    }
}