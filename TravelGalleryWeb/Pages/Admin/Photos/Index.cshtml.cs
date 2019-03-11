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
    public class IndexModel : PageModel
    {
        private readonly ApplicationContext _context;
 
        public IList<DisplayPhoto> Photos { get;set; }
        
        public List<SelectListItem> AlbumsList; 
        
        [BindProperty(SupportsGet = true)] public int Id { get; set; }
        public string AlbumName { get; set; } = "--";
        [BindProperty(SupportsGet = true)] public int PageNumber { get; set; }
        private const int PageSize = 15;
        public int PagesTotal { get; set; }

        public IndexModel(ApplicationContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            var albums = await _context.Albums.AsNoTracking().Select(a => new{Id = a.Id, Name = a.Name}).OrderBy(a => a.Name).ToListAsync();
            
            AlbumsList = albums
                .Select(album => new SelectListItem {
                    Value = album.Id.ToString(),
                    Text = album.Name
                }).ToList();
            
            ViewData["Albums"] = AlbumsList;
            Photos = new List<DisplayPhoto>();

            if (albums.Any())
            {
                if (albums.All(a => a.Id != Id))
                {
                    Id = albums[0].Id;
                }

                AlbumName = albums.First(a => a.Id == Id).Name;

                CalculatePages();

                if (PageNumber < 1 || PageNumber > PagesTotal)
                {
                    PageNumber = 1;}

                Photos = await _context.Photos.AsNoTracking().Where(p => p.AlbumId == Id).Join(_context.Albums,
                    p => p.AlbumId,
                    a => a.Id,
                    (p, a) => new DisplayPhoto()
                    {
                        Id = p.Id,
                        AlbumName = p.Album.Name,
                        Comment = p.Comment,
                        FullPath = p.FullPath,
                        Tag = p.Tag,
                        Year = p.Year
                    }).Skip((PageNumber -1) * PageSize).Take(PageSize).ToListAsync();
            }
        }

        private void CalculatePages()
        {
            var photosInAlbumTotal = _context.Photos.AsNoTracking().Count(p => p.AlbumId == Id);
            PagesTotal = photosInAlbumTotal / PageSize;
            if (photosInAlbumTotal % PageSize != 0)
            {
                PagesTotal++;
            }
        }
    }
}