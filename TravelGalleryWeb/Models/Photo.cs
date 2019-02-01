using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TravelGalleryWeb.Models
{
    public class Photo
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string FullPath { get; set; }

        [Required]
        public int Year { get; set; }
        
        [Required]
        public PhotoTag Tag { get; set; }
        
        [Required]
        public Album Album { get; set; }

        public string Comment { get; set; }
    }
}