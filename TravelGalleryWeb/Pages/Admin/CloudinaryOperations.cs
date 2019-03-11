using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace TravelGalleryWeb.Pages.Admin
{
    public class CloudinaryOperations : IStorageOperations
    {
        private readonly Cloudinary _cloudinary;
        private readonly IOptions<Constants> _options;
        
        public CloudinaryOperations(IOptions<Constants> options)
        {
            string Link = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
            var accountUri = new Uri(Link);
            var userInfo = accountUri.UserInfo.Split(':');
            
            Account account = new Account();
            account.Cloud = accountUri.Host;
            account.ApiKey = userInfo[0];
            account.ApiSecret = userInfo[1];
            _cloudinary = new Cloudinary(account);
            _options = options;
        }

        public string Upload(string path, bool IsCover)
        {
            if (path == null) return null;
            
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(path),
                Transformation = new Transformation().Height(IsCover ? _options.Value.CoverHeight : _options.Value.PhotoHeight)
            };
            
            var uploadResult = _cloudinary.Upload(uploadParams);
            return uploadResult.SecureUri.ToString();
        }

        //delete a number of images
        public void Delete(List<string> paths)
        {
            if (!paths.Any()) return;
            List<string> ids = new List<string>();
            foreach (var path in paths)
            {
                string id = Path.GetFileNameWithoutExtension(path);
                ids.Add(id);
            }
            
            var delParams = new DelResParams()
            {
                PublicIds = ids,
                Invalidate = true
            };
            
            _cloudinary.DeleteResources(delParams);
        }
        
        //delete one image
        public void Delete(string path)
        {
            if(path==null) return;
            List<string> ids = new List<string>();
            string id = Path.GetFileNameWithoutExtension(path);
            ids.Add(id);
            
            var delParams = new DelResParams()
            {
                PublicIds = ids,
                Invalidate = true
            };
            
            _cloudinary.DeleteResources(delParams);
        }
    }
}