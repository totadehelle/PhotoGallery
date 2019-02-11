using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Albums
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationContext _context;
        private readonly IHostingEnvironment _appEnvironment;

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

                Album.Cover = imagePath;
            }
            
            _context.Albums.Add(Album);
                
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}