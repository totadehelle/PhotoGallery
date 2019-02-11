using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelGalleryWeb.Models
{
    public class Photo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Display(Name = "Full path")]
        public string FullPath { get; set; }

        [Required]
        public int Year { get; set; }
        
        [Required]
        public PhotoTag Tag { get; set; }
        
        public Album Album { get; set; }

        [Display(Name = "Album")]
        public int AlbumId { get; set; }

        public string Comment { get; set; }
    }
}