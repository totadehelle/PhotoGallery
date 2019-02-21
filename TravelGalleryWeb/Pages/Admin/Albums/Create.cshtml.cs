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
    
    public class CreateModel : PageModel
    {
        private readonly ApplicationContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        public string Message { get; set; }  = "It is recommended to use square images for the cover.";

        public CreateModel(ApplicationContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Album Album { get; set; }

        
        public async Task<IActionResult> OnPostAsync(IFormCollection CoverImage)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (AlbumExists(Album.Name))
            {
                Message = "Album with this name already exists, please choose other name.";
                return Page();
            }
            
            var file = CoverImage.Files.FirstOrDefault();

            if (file != null)
            {
                var newFileName = Guid.NewGuid().ToString() + "_" +
                                  Path.GetFileName(file.FileName);
                var imagePath = @"/uploadedFiles/" + newFileName;
                string resizedImagePath = @"/resizedFiles/" + "r_" + newFileName;


                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + imagePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                
                ImageProcessor.Resize(_appEnvironment.WebRootPath + imagePath, _appEnvironment.WebRootPath + resizedImagePath,true);

                Album.Cover = resizedImagePath;
            }
            
            _context.Albums.Add(Album);
                
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        
        private bool AlbumExists(string name)
        {
            return _context.Albums.AsNoTracking().Any(e => e.Name == name);
        }
    }
}