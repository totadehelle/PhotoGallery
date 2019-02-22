using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Photos
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationContext _context;
        private readonly IHostingEnvironment _appEnvironment;

        public DeleteModel(ApplicationContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        [BindProperty]
        public Photo Photo { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Photo = await _context.Photos.FindAsync(id);

            if (Photo != null)
            {
                if (System.IO.File.Exists(_appEnvironment.WebRootPath + Photo.FullPath))
                {
                    System.IO.File.Delete(_appEnvironment.WebRootPath + Photo.FullPath);
                }

                var previewPath = _appEnvironment.WebRootPath +
                                  Photo.FullPath.Replace("resizedFiles/r_", "previewFiles/p_");
                
                if (System.IO.File.Exists(previewPath))
                {
                    System.IO.File.Delete(previewPath);
                }
                _context.Photos.Remove(Photo);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
