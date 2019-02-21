using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using SixLabors.ImageSharp.PixelFormats;

namespace TravelGalleryWeb.Pages.Admin
{
    public static class ImageProcessor
    {
        private const int ImageHeight = 900;
        private const int CoverHeight = 350;
        public static void Resize(string originPath, string resizedPath, bool isAlbumCover)
        {
            using (Image<Rgba32> image = Image.Load(originPath))
            {
                if (image == null) return;

                var height = isAlbumCover ? CoverHeight : ImageHeight;

                if (image.Height > height)
                {
                    float coefficient = (float)image.Height / (float)height;
                    int width = (int)(image.Width / coefficient);
                    image.Mutate(x => x
                        .Resize(width, height));
                    image.Save(resizedPath); // Automatic encoder selected based on extension.
                }
                
            }    
        }
    }
}