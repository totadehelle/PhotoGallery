using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelGalleryWeb.Models
{
    public class Album
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Display(Name = "Changed Name!!")]
        public string Name { get; set; }
        
        public IList<Photo> Photos { get; set; }

        public string Cover { get; set; }
    }
}