using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Admins
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationContext _context;
        
        public DeleteModel(ApplicationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Admin Admin { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Admin = await _context.Admins.FirstOrDefaultAsync(m => m.Id == id);

            if (Admin == null)
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

            Admin = await _context.Admins.FindAsync(id);

            if (Admin != null)
            {
                _context.Admins.Remove(Admin);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
