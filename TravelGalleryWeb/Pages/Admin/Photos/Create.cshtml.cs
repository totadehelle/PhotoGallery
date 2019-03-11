using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Photos
{
    [RequestSizeLimit(100_000_000)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IOptions<Constants> _config;
        private readonly ImageProcessor _processor;
        public string Message { get; set; }
        
        public List<SelectListItem> AlbumsList => _context.Albums.AsNoTracking()
            .Select(album => new SelectListItem {
                Value = album.Id.ToString(),
                Text = album.Name
            }).ToList();

        public CreateModel(ApplicationContext context, IHostingEnvironment appEnvironment, IOptions<Constants> config, IStorageOperations storage)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _config = config;
            _processor = new ImageProcessor(storage);
        }

        public void OnGet()
        {
            ViewData["Albums"] = AlbumsList;
        }

        [BindProperty]
        public Photo Photo { get; set; }

        public async Task<IActionResult> OnPostAsync(IFormFileCollection Images)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Images == null) return RedirectToPage("./Index");
            
            foreach (var image in Images)
            {
                if (Path.GetExtension(image.FileName).ToLower() != ".jpeg"
                    && Path.GetExtension(image.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(image.FileName).ToLower() != ".gif"
                    && Path.GetExtension(image.FileName).ToLower() != ".bmp"
                    && Path.GetExtension(image.FileName).ToLower() != ".png")
                {
                    Message = "Wrong image files format, possible formats are: PNG, GIF, BMP, JPEG, JPG.";
                    return Page();
                }
                    
                var newFileName = Guid.NewGuid().ToString() + Path.GetFileName(image.FileName);
                var imagePath = _appEnvironment.WebRootPath + _config.Value.UploadDir + newFileName;
                
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
                    
                var link = _processor.Upload(imagePath, false);
                System.IO.File.Delete(imagePath);
                
                var newPhoto = new Photo()
                {
                    Album = Photo.Album, 
                    Id = Photo.Id, 
                    Tag = Photo.Tag, 
                    Year = Photo.Year, 
                    Comment = Photo.Comment, 
                    AlbumId = Photo.AlbumId, 
                    FullPath = link
                };
                    
                _context.Photos.Add(newPhoto);
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}