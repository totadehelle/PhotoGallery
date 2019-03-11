using System;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TravelGalleryWeb.Models;

namespace TravelGalleryWeb.Data
{
    public class ApplicationContext  : DbContext
    {
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Admin> Admins { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Album>()
                .HasMany(e => e.Photos)
                .WithOne(e => e.Album)
                .HasForeignKey(e => e.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Photo>().HasIndex(u => u.Year);
            modelBuilder.Entity<Admin>().HasIndex(u => u.Login);
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
            };
            
            var connectionString = builder.ToString();
            optionsBuilder.UseNpgsql(connectionString);
        }
        
    }
}