using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public void OnGet()
        {
            var a =_context.Database.GetDbConnection().ConnectionString;
            AlbumList = _context.Albums.AsNoTracking().Select(p => p).ToList();
        }
    }
}