using System.Collections.Generic;
using System.Linq;
using TravelGalleryWeb.Data;
using TravelGalleryWeb.Models;

namespace TravelGalleryWeb.Pages.Admin
{
    public class ImageProcessor
    {
        public ImageProcessor(IStorageOperations storage)
        {
            _storage = storage;
        }
        
        IStorageOperations _storage;
        
        public string Upload(string path, bool isCover)
        {
            return _storage.Upload(path, isCover);
        }
        
        public void DeleteAlbumFiles(Album album, ApplicationContext context)
        {
            if (album == null) return;
            _storage.Delete(album.Cover);
            List<string> relatedPhotos = context.Photos.Where(p => p.AlbumId == album.Id).Select(p => p.FullPath).ToList();
            _storage.Delete(relatedPhotos);
        }
        
        public void DeleteImage(string path)
        {
            if (path == null) return;
            _storage.Delete(path);
        }
    }
}