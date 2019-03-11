using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Photos
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IOptions<Constants> _config;
        private readonly ImageProcessor _processor;

        public DeleteModel(ApplicationContext context, IHostingEnvironment appEnvironment,IOptions<Constants> config, IStorageOperations storage)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _config = config;
            _processor = new ImageProcessor(storage);
        }

        [BindProperty]
        public Photo Photo { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Photo = await _context.Photos.FirstOrDefaultAsync(m => m.Id == id);

            if (Photo == null)
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

            Photo = await _context.Photos.FindAsync(id);

            if (Photo == null) return RedirectToPage("./Index");
            
            _processor.DeleteImage(Photo.FullPath);
            _context.Photos.Remove(Photo);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
