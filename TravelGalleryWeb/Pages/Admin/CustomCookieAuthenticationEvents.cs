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
        private readonly ApplicationContext _userRepository;

        public CustomCookieAuthenticationEvents(ApplicationContext userRepository)
        {
            // Get the database from registered DI services.
            _userRepository = userRepository;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;
            
            var name = (from c in userPrincipal.Claims
                where c.Type == ClaimTypes.Name
                select c.Value).FirstOrDefault();

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
            var targetUser = await _userRepository.Admins.AsNoTracking().FirstOrDefaultAsync(t => t.Login == name);
            return targetUser != null && targetUser.LastChanged.ToString() == lastChanged;
        }
    }

}