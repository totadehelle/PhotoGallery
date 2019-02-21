using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Data;
using TravelGalleryWeb.Models;

namespace TravelGalleryWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationContext _context;
        public List<Album> AlbumList;

        public IndexModel(ApplicationContext context)
        {
            _context = context;
        }
        
        public async Task OnGetAsync()
        {
            AlbumList = await _context.Albums.AsNoTracking().Select(p => p).ToListAsync();
        }
    }
}