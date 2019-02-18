using System.ComponentModel.DataAnnotations;

namespace TravelGalleryWeb.Models
{
    public class DisplayPhoto
    {
        public int Id;
        
        [Display(Name = "Preview")]
        public string FullPath;
        
        public PhotoTag Tag;
        
        public string Comment;
        
        public int Year;
        
        [Display(Name = "Album")]
        public string AlbumName;
    }
}