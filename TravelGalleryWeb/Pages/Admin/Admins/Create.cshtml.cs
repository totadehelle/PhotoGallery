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
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Admins
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationContext _context;

        public CreateModel(ApplicationContext context)
        {
            _context = context;
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

            Admin.Password = EncryptionTools.HashPassword(Admin.Password);
            
            _context.Admins.Add(Admin);
                
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}