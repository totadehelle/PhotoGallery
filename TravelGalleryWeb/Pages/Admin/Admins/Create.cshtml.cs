using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Admins
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationContext _context;
        private readonly EncryptionTools _encryption;
        public string Message { get; set; }

        public CreateModel(ApplicationContext context, IOptions<Constants> config)
        {
            _context = context;
            _encryption = new EncryptionTools(config);
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Models.Admin Admin { get; set; }

        public async Task<IActionResult> OnPostAsync(IFormCollection CoverImage)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (AdminExists(Admin.Login))
            {
                Message = "User with this login already exists, please choose other login.";
                return Page();
            }

            Message = null;

            Admin.Password = _encryption.HashPassword(Admin.Password);
            Admin.LastChanged = DateTime.Now.ToUniversalTime();
            
            await _context.Admins.AddAsync(Admin);
                
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        
        private bool AdminExists(string login)
        {
            return _context.Admins.AsNoTracking().Any(e => e.Login == login);
        }
    }
}