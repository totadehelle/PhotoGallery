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
using SQLitePCL;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Photos
{
    [RequestSizeLimit(bytes: 100_000_000)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        
        public List<SelectListItem> AlbumsList => _context.Albums.AsNoTracking()
            .Select(album => new SelectListItem {
                Value = album.Id.ToString(),
                Text = album.Name
            }).ToList();

        public CreateModel(ApplicationContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
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
            
            if(Images != null){
                foreach (var image in Images)
                {
                    string newFileName = Guid.NewGuid().ToString() + "_" +
                                         Path.GetFileName(image.FileName);
                    string imagePath = @"/uploadedFiles/" + newFileName;
                    string resizedImagePath = @"/resizedFiles/" + "r_" + newFileName;


                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + imagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                    
                    ImageProcessor.Resize(_appEnvironment.WebRootPath + imagePath,_appEnvironment.WebRootPath + resizedImagePath,false);

                    var newPhoto = new Photo()
                    {
                        Album = Photo.Album, 
                        Id = Photo.Id, 
                        Tag = Photo.Tag, 
                        Year = Photo.Year, 
                        Comment = Photo.Comment, 
                        AlbumId = Photo.AlbumId, 
                        FullPath = resizedImagePath
                    };
                    
                    _context.Photos.Add(newPhoto);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}