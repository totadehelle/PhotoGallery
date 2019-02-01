using System.ComponentModel.DataAnnotations;

namespace TravelGalleryWeb.Models
{
    public class Album
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
    }
}