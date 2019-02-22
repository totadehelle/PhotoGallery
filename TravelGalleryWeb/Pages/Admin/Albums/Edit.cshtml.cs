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
    public class EditModel : PageModel
    {
        private readonly ApplicationContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IOptions<Constants> _config;
        private readonly ImageProcessor _processor;
        public string Message { get; set; }  = "It is recommended to use square images for the cover.";

        public EditModel(ApplicationContext context, IHostingEnvironment appEnvironment, IOptions<Constants> config)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _config = config;
            _processor = new ImageProcessor(_config);
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
            
            if (AlbumExists(Album.Name, Album.Id))
            {
                Message = "Album with this name already exists, please choose other name.";
                return Page();
            }
            
            var file = CoverImage.Files.FirstOrDefault();

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
                
                _processor.Resize(_appEnvironment.WebRootPath + imagePath, _appEnvironment.WebRootPath + resizedImagePath,
                    true, false);
                
                if (System.IO.File.Exists(_appEnvironment.WebRootPath + Album.Cover))
                {
                    System.IO.File.Delete(_appEnvironment.WebRootPath + Album.Cover);
                }

                Album.Cover = resizedImagePath;
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
        
        private bool AlbumExists(string name, int id)
        {
            return _context.Albums.AsNoTracking().Any(e => e.Name == name && e.Id != id);
        }
    }
}
