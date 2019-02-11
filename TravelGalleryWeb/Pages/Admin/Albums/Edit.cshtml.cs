using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Albums
{
    public class EditModel : PageModel
    {
        private readonly ApplicationContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        

        public EditModel(ApplicationContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
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

        public async Task<IActionResult> OnPostAsync(IFormCollection CoverImage)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var file = CoverImage.Files.FirstOrDefault();

            if (file != null)
            {
                var newFileName = Guid.NewGuid().ToString() + "_" +
                                  Path.GetFileName(file.FileName);
                var imagePath = @"/uploadedFiles/" + newFileName;


                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + imagePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                
                if (System.IO.File.Exists(_appEnvironment.WebRootPath + Album.Cover))
                {
                    System.IO.File.Delete(_appEnvironment.WebRootPath + Album.Cover);
                }

                Album.Cover = imagePath;
            }

            _context.Attach(Album).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlbumExists(Album.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AlbumExists(int id)
        {
            return _context.Albums.Any(e => e.Id == id);
        }
    }
}
