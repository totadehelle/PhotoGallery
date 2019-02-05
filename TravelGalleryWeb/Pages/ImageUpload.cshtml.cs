using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using TravelGalleryWeb.Repositories;

namespace TravelGalleryWeb.Pages
{
    public class ImageUploadModel : PageModel
    {
        public ImageUploadModel(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }
        
        private ApplicationContext _context;
        IHostingEnvironment _appEnvironment;
        public object Message { get; set; }


        public void OnGet()
        {
            Message = "Please choose a file to upload";
        }

     

        public async void OnPostAsync(IFormFile Image)
        {
            if(Image != null){
                string newFileName = Guid.NewGuid().ToString() + "_" +
                              Path.GetFileName(Image.FileName);
                string imagePath = @"/uploadedFiles/" + newFileName;


                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + imagePath, FileMode.Create))
                {
                    await Image.CopyToAsync(fileStream);
                    
                }
                
                Message = "Your file is uploaded!";
                
                //to add saving to the database and so on
            }
            else
            {
                Message = "No file was been chosen.";
            }
        }
    }
}