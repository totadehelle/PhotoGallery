using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Data;
using TravelGalleryWeb.Models;

namespace TravelGalleryWeb.Pages.Admin.Albums
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationContext _context;

        public DetailsModel(ApplicationContext context)
        {
            _context = context;
        }

        public Album Album { get; set; }
        public int NumberOfPhotos { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Album = await _context.Albums.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

            if (Album == null)
            {
                return NotFound();
            }

            NumberOfPhotos = _context.Photos.AsNoTracking().Count(p => p.AlbumId == Album.Id);
            
            return Page();
        }
    }
}
