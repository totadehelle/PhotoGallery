using Microsoft.EntityFrameworkCore;
using TravelGalleryWeb.Models;

namespace TravelGalleryWeb.Data
{
    public class ApplicationContext  : DbContext
    {
       
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Admin> Admins { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Album>()
                .HasMany(e => e.Photos)
                .WithOne(e => e.Album)
                .HasForeignKey(e => e.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Photo>().HasIndex(u => u.Year);
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"DataSource=TravelGalleryContext.db;");
        }
        
    }
}