using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Data;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Pages.Admin.Admins;

namespace TravelGalleryWeb.Pages.Admin
{
    public class SigninModel : PageModel
    {
        private ApplicationContext _context;
        public AuthStatus Status;
        public string UserName;

        [BindProperty]
        public Models.Admin Admin { get; set; }
        
        public enum AuthStatus
        {
            Authorized,
            LoggedOut,
            Error,
        }

        public SigninModel(ApplicationContext context)
        {
            _context = context;
            
        }
        
        public void OnGet()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                UserName = HttpContext.User.Identity.Name;
                Status = AuthStatus.Authorized;
            }
            else
            {
                Status = AuthStatus.LoggedOut;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var targetUser = _context.Admins.AsNoTracking().FirstOrDefault(t => t.Login == Admin.Login);

            if (targetUser == null)
            {
                Status = AuthStatus.Error;
                return Page();
            }
            
            if (!EncryptionTools.VerifyPassword(Admin.Password, targetUser.Password))
            {
                Status = AuthStatus.Error;
                return Page();
            }

            Admin.Role = targetUser.Role;
            Admin.LastChanged = targetUser.LastChanged;
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(GetIdentity()), new AuthenticationProperties{IsPersistent = true});
            UserName = Admin.Login;
            Status = AuthStatus.Authorized;
            return RedirectToPage("./Index");
        }

        private ClaimsIdentity GetIdentity()
        {
            var claims = new List<Claim>();
            
            claims.Add(new Claim(ClaimTypes.Name, Admin.Login));
            switch (Admin.Role)
            {
                case Role.Administrator:
                    claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
                    break;
                case Role.ContentManager:
                    claims.Add(new Claim(ClaimTypes.Role, "ContentManager"));
                    break;
            }
            claims.Add(new Claim("LastChanged", Admin.LastChanged.ToString(CultureInfo.CurrentCulture)));
            var claimsIdentity =
                new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return claimsIdentity;
        }
    }
}