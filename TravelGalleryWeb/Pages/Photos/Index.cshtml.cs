using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Repositories;

namespace TravelGallery.Pages.Photos
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationContext _context;

        public IndexModel(ApplicationContext context)
        {
            _context = context;
        }

        public IList<Photo> Photos { get;set; }

        public async Task OnGetAsync()
        {
            Photos = await _context.Photos.ToListAsync();
        }
    }
}
