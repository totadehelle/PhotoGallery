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
            _previewWidth = config.Value.PreviewWidth;
        }

        private readonly int _imageHeight;
        private readonly int _coverHeight;
        private readonly int _previewWidth;
        public void Resize(string originPath, string resizedPath, bool isAlbumCover, bool isPreview)
        {
            using (var image = Image.Load(originPath))
            {
                if (image == null) return;

                if(isPreview) ChangeWidth(image);
                else ChangeHeight(image, isAlbumCover);
                image.Save(resizedPath); // Automatic encoder selected based on extension.
            }    
        }

        private void ChangeHeight(Image<Rgba32> image, bool isAlbumCover)
        {
            var height = isAlbumCover ? _coverHeight : _imageHeight;

            if (image.Height <= height) return;
            var coefficient = (float)image.Height / (float)height;
            var width = (int)(image.Width / coefficient);
            image.Mutate(x => x
                .Resize(width, height));
        }
        
        private void ChangeWidth(Image<Rgba32> image)
        {
            if (image.Width <= _previewWidth) return;
            var coefficient = (float)image.Width / (float)_previewWidth;
            var height = (int)(image.Height / coefficient);
            image.Mutate(x => x
                .Resize(_previewWidth, height));
        }
    }
}