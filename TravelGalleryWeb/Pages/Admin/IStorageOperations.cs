using System.Collections.Generic;

namespace TravelGalleryWeb.Pages.Admin
{
    public interface IStorageOperations
    {
        string Upload(string path, bool isCover);
        //delete a number of images with previews
        void Delete(List<string> paths);
        //delete one image without a preview (album cover)
        void Delete(string path);
    }
}