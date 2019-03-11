using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelGalleryWeb.Data;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Pages.Admin.Admins;

namespace TravelGalleryWeb.Pages.Admin
{
    public class SigninModel : PageModel
    {
        private ApplicationContext _context;
        private readonly EncryptionTools _encryption;
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

        public SigninModel(ApplicationContext context, IOptions<Constants> config)
        {
            _context = context;
            _encryption = new EncryptionTools(config);
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
            var targetUser = await _context.Admins.AsNoTracking().FirstOrDefaultAsync(t => t.Login == Admin.Login);

            if (targetUser == null)
            {
                Status = AuthStatus.Error;
                return Page();
            }
            
            if (!_encryption.VerifyPassword(Admin.Password, targetUser.Password))
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