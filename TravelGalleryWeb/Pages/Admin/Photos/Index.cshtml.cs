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

        public List<Album> Albums;
        
        public List<SelectListItem> AlbumsList => Albums
            .Select(album => new SelectListItem {
                Value = album.Id.ToString(),
                Text = album.Name
            }).ToList();
        
        public IList<DisplayPhoto> Photos { get;set; }

        [BindProperty(SupportsGet = true)] public int Id { get; set; }
        public string AlbumName { get; set; } = "--";

        private const int PageSize = 5;
        private int StartFrom = 0;

        public int PageCurrent { get; set; } = 1;
        public int PagesTotal { get; set; }

        public IndexModel(ApplicationContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Albums = await _context.Albums.AsNoTracking().Select(a => a).OrderBy(a => a.Name).ToListAsync();
            
            ViewData["Albums"] = AlbumsList;
            Photos = new List<DisplayPhoto>();

            if (Albums.Any())
            {
                if (Albums.All(a => a.Id != Id))
                {
                    Id = Albums[0].Id;
                }

                AlbumName = Albums.FirstOrDefault(a => a.Id == Id).Name;

                CalculatePages();

                Photos = await _context.Photos.AsNoTracking().Where(p => p.AlbumId == Id).Join(_context.Albums,
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
                    }).Skip(StartFrom).Take(PageSize).ToListAsync();
            }
        }

        public async Task OnGetNextAsync()
        {
            StartFrom += PageSize;
            await OnGetAsync();
            PageCurrent++;
        }
        
        public async Task OnGetPreviousAsync()
        {
            StartFrom -= PageSize;
            await OnGetAsync();
            PageCurrent--;
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