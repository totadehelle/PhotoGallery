using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Albums
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationContext _context;

        public IndexModel(ApplicationContext context)
        {
            _context = context;
        }

        public IList<Album> Albums { get;set; }

        public async Task OnGetAsync()
        {
            Albums = await _context.Albums.ToListAsync();
        }
    }
}
