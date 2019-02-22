using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Photos
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationContext _context;
        private readonly IOptions<Constants> _congig;
        public string originPath { get; set; }

        public DetailsModel(ApplicationContext context, IOptions<Constants> congig)
        {
            _context = context;
            _congig = congig;
        }

        public Photo Photo { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Photo = await _context.Photos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

            if (Photo == null)
            {
                return NotFound();
            }

            originPath = Photo.FullPath.Replace(_congig.Value.ResizedDir+_congig.Value.ResizedPrefix, _congig.Value.UploadDir);
            
            return Page();
        }
    }
}
