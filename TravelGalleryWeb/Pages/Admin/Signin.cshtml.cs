using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin
{
    public class SigninModel : PageModel
    {
        private const string Salt = "3769011ffb974839a44d2110f9683bf7";
        private ApplicationContext _context;
        
        public enum AuthStatus
        {
            Authorized,
            LoggedOut,
            Error,
        }

        
        public AuthStatus Status;
        public string UserName;

        [BindProperty]
        public Models.Admin Admin { get; set; }
        
        

        public SigninModel(ApplicationContext context)
        {
            _context = context;
            
        }
        
        public void OnGet()
        {
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                UserName = HttpContext.User.Identity.Name;
                Status = AuthStatus.Authorized;
            }
            else
            {
                Status = AuthStatus.LoggedOut;
            }
        }

        public async Task OnPostSignInAsync()
        {
            var targetUser = _context.Admins.FirstOrDefault(t => t.Login == Admin.Login);

            if (targetUser == null)
            {
                Status = AuthStatus.Error;
                return;
            }
            
            if (!_verifyPassword(Admin.Password, targetUser.Password))
            {
                Status = AuthStatus.Error;
                return;
            }

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(GetIdentity()), new AuthenticationProperties{IsPersistent = true});
            UserName = Admin.Login;
            Status = AuthStatus.Authorized;
        }

        public async void OnPostSignOutAsync()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
                Status = AuthStatus.LoggedOut;
            }
        }

        private ClaimsIdentity GetIdentity()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Admin.Login),
                new Claim(ClaimTypes.Role, "Administrator"),
            };
            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return claimsIdentity;
        }
        
        private string _hashPassword(string password)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(password+Salt));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        
        private bool _verifyPassword(string input, string hash)
        {
            string hashOfInput = _hashPassword(input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }
}