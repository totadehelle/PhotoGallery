using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.PixelFormats;

namespace TravelGalleryWeb.Pages.Admin
{
    public class ImageProcessor
    {
        public ImageProcessor(IOptions<Constants> config)
        {
            _imageHeight = config.Value.PhotoHeight;
            _coverHeight = config.Value.CoverHeight;
        }

        private readonly int _imageHeight;
        private readonly int _coverHeight;
        public void Resize(string originPath, string resizedPath, bool isAlbumCover)
        {
            using (var image = Image.Load(originPath))
            {
                if (image == null) return;

                var height = isAlbumCover ? _coverHeight : _imageHeight;

                if (image.Height <= height) return;
                var coefficient = (float)image.Height / (float)height;
                var width = (int)(image.Width / coefficient);
                image.Mutate(x => x
                    .Resize(width, height));
                image.Save(resizedPath); // Automatic encoder selected based on extension.
            }    
        }
    }
}