using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Photos
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationContext _context;

        public IndexModel(ApplicationContext context)
        {
            _context = context;
        }

        public IList<DisplayPhoto> Photos { get;set; }

        public async Task OnGetAsync()
        {
            Photos = await _context.Photos.Join(_context.Albums,
                p => p.AlbumId,
                a => a.Id,
                (p, a) => new DisplayPhoto()
                {
                    Id = p.Id,
                    FullPath = p.FullPath, 
                    Tag = p.Tag, 
                    Comment = p.Comment, 
                    Year = p.Year, 
                    AlbumName = a.Name
                }).ToListAsync();
        }
    }
}
