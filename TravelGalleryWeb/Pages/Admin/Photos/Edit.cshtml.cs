using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Photos
{
    public class EditModel : PageModel
    {
        private readonly ApplicationContext _context;
        
        public List<SelectListItem> AlbumsList => _context.Albums
            .Select(album => new SelectListItem {
                Value = album.Id.ToString(),
                Text = album.Name
            }).ToList();

        public EditModel(ApplicationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Photo Photo { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ViewData["Albums"] = AlbumsList;
            
            if (id == null)
            {
                return NotFound();
            }

            Photo = await _context.Photos.FirstOrDefaultAsync(m => m.Id == id);

            if (Photo == null)
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

            _context.Attach(Photo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhotoExists(Photo.Id))
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

        private bool PhotoExists(int id)
        {
            return _context.Photos.Any(e => e.Id == id);
        }
    }
}
