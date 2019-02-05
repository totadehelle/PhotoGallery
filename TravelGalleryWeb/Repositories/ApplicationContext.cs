using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Models;

namespace TravelGalleryWeb.Repositories
{
    public class ApplicationContext  : DbContext
    {
       
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Album> Albums { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Album>()
                .HasMany(e => e.Photos)
                .WithOne(e => e.Album)
                .HasForeignKey(e => e.AlbumName)
                .OnDelete(DeleteBehavior.Cascade);
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"DataSource=TravelGalleryContext.db;");
        }
        
    }
}