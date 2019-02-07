using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages
{
    public class AlbumModel : PageModel
    {
        private ApplicationContext _context;
        
        [BindProperty(SupportsGet = true)]
        public string Location { get; set; }

        public List<Photo> PhotoCollection { get; set; }
        
        public List<int> Years { get; set; }

        public AlbumModel(ApplicationContext context)

        {

            _context = context;

        }
        
        public async Task OnGet()
        {
            int id = _context.Albums.FirstOrDefaultAsync(a => a.Name == Location).Result.Id;
            
            PhotoCollection = await _context.Photos.Where(p => p.AlbumId == id).ToListAsync();

            Years = (from photo in PhotoCollection select photo.Year).Distinct().OrderByDescending(year => year).ToList();
        }
    }
}