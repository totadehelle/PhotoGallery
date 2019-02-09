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
using SQLitePCL;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages.Admin.Photos
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationContext _context;
        IHostingEnvironment _appEnvironment;
        
        public List<SelectListItem> AlbumsList => _context.Albums
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


                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + imagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    Photo newPhoto = new Photo()
                    {
                        Album = Photo.Album, 
                        Id = Photo.Id, 
                        Tag = Photo.Tag, 
                        Year = Photo.Year, 
                        Comment = Photo.Comment, 
                        AlbumId = Photo.AlbumId, 
                        FullPath = imagePath
                    };
                    
                    _context.Photos.Add(newPhoto);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}