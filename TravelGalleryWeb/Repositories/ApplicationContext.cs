using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Models;

namespace TravelGalleryWeb.Repositories
{
    public class ApplicationContext  : DbContext
    {
       
        public DbSet<Photo> Photos { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        { }
        
    }
}