using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Albums
{
    
    public class CreateModel : PageModel
    {
        private readonly ApplicationContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IOptions<Constants> _config;
        private readonly ImageProcessor _processor;
        public string Message { get; set; }  = "It is recommended to use square images for the cover.";

        public CreateModel(ApplicationContext context, IHostingEnvironment appEnvironment, IOptions<Constants> config)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _config = config;
            _processor = new ImageProcessor(_config);
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Album Album { get; set; }

        
        public async Task<IActionResult> OnPostAsync(IFormCollection coverImage)
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
            
            var file = coverImage.Files.FirstOrDefault();

            if (file != null)
            {
                if (Path.GetExtension(file.FileName).ToLower() != ".jpeg"
                    && Path.GetExtension(file.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(file.FileName).ToLower() != ".gif"
                    && Path.GetExtension(file.FileName).ToLower() != ".bmp"
                    && Path.GetExtension(file.FileName).ToLower() != ".png")
                {
                    Message = "Wrong image file format, possible formats are: PNG, GIF, BMP, JPEG, JPG.";
                    return Page();
                }
                
                var newFileName = Guid.NewGuid().ToString() + "_" +
                                  Path.GetFileName(file.FileName);
                var imagePath = _config.Value.UploadDir + newFileName;
                var resizedImagePath = _config.Value.ResizedDir + _config.Value.ResizedPrefix + newFileName;

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + imagePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                
                _processor.Resize(_appEnvironment.WebRootPath + imagePath, _appEnvironment.WebRootPath + resizedImagePath,true);

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