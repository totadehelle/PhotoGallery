using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelGalleryWeb.Models
{
    public class Album
    {
        [Key]
        public string Name { get; set; }
        
        public IList<Photo> Photos { get; set; }
    }
}