﻿
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Admins
{
    public class EditModel : PageModel
    {
        private readonly ApplicationContext _context;
        private readonly EncryptionTools _encryption;
        
        public EditModel(ApplicationContext context, IOptions<Constants> config)
        {
            _context = context;
            _encryption = new EncryptionTools(config);
        }

        [BindProperty]
        public Models.Admin Admin { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Admin = await _context.Admins.FirstOrDefaultAsync(m => m.Id == id);

            if (Admin == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Admin.Password = _encryption.HashPassword(Admin.Password);
            Admin.LastChanged = DateTime.UtcNow;
            
            _context.Attach(Admin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminExists(Admin.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.Id == id);
        }
    }
}
