using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin
{
    public class CustomCookieAuthenticationEvents: CookieAuthenticationEvents
    {
        private readonly ApplicationContext _context;

        public CustomCookieAuthenticationEvents(ApplicationContext context)
        {
            _context = context;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;
            
            var name = (from claim in userPrincipal.Claims
                where claim.Type == ClaimTypes.Name
                select claim.Value).FirstOrDefault();

            // Look for the LastChanged claim.
            var lastChanged = (from c in userPrincipal.Claims
                where c.Type == "LastChanged"
                select c.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(lastChanged) ||
                !ValidateLastChanged(lastChanged, name).Result)
            {
                context.RejectPrincipal();

                await context.HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }

        private async Task<bool> ValidateLastChanged(string lastChanged, string name)
        {
            var targetUser = await _context.Admins.AsNoTracking().FirstOrDefaultAsync(t => t.Login == name);
            return targetUser != null && targetUser.LastChanged.ToString() == lastChanged;
        }
    }

}