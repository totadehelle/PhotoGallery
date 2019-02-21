using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Albums
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
        public Album Album { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Album = await _context.Albums.FirstOrDefaultAsync(m => m.Id == id);

            if (Album == null)
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

            Album = await _context.Albums.FindAsync(id);

            if (Album != null)
            {
                if (System.IO.File.Exists(_appEnvironment.WebRootPath + Album.Cover))
                {
                    System.IO.File.Delete(_appEnvironment.WebRootPath + Album.Cover);
                }
                
                string[] relatedPhotos = _context.Photos.Where(p => p.AlbumId == Album.Id).Select(p => p.FullPath).ToArray();

                foreach (var path in relatedPhotos)
                {
                    if (System.IO.File.Exists(_appEnvironment.WebRootPath + path))
                    {
                        System.IO.File.Delete(_appEnvironment.WebRootPath + path);
                    }
                }
                
                _context.Albums.Remove(Album);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
