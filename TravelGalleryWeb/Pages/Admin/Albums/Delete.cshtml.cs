using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Albums
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationContext _context;
        
        private readonly IOptions<Constants> _config;
        private readonly ImageProcessor _processor;
        

        public DeleteModel(ApplicationContext context, IOptions<Constants> config, IStorageOperations storage)
        {
            _context = context;
            _config = config;
            _processor = new ImageProcessor(storage);
        }

        [BindProperty]
        public Album Album { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Album = await _context.Albums.FirstOrDefaultAsync(m => m.Id == id);

            if (Album == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Album = await _context.Albums.FindAsync(id);

            if (Album == null) return RedirectToPage("./Index");
            
            _processor.DeleteAlbumFiles(Album, _context);
            _context.Albums.Remove(Album);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
