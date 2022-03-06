using GalleryApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GalleryApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }
        public DbSet<StoredFile> Files { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Thumbnail> Thumbnails { get; set; }       
        public DbSet<GalleryStoredFile> GalleryStoredFiles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Thumbnail>().HasKey(t => new { t.FileId, t.Type });
            modelBuilder.Entity<GalleryStoredFile>().HasKey(t => new { t.StoredFileId, t.GalleryId });
            modelBuilder.Entity<GalleryStoredFile>()
                .HasOne(g => g.Gallery)
                .WithMany(g => g.StoredPictures)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<GalleryStoredFile>()
                .HasOne(s => s.StoredFile)
                .WithMany(gs => gs.Galleries)
                .OnDelete(DeleteBehavior.Restrict);

            var hasher = new PasswordHasher<AppUser>();
            AppUser lukram = new AppUser
            {
                Id = "fc7a69e4-c851-4eca-ba64-ac182cc4fc95",
                Email = "lukkram019@pslib.cz",
                NormalizedEmail = "LUKKRAM019@PSLIB.CZ",
                EmailConfirmed = true,
                UserName = "lukkram019@pslib.cz",
                NormalizedUserName = "LUKKRAM019@PSLIB.CZ",
                LockoutEnabled = false,
                SecurityStamp = string.Empty,
                PasswordHash = hasher.HashPassword(null, "Heslo"),
                NickName = "Aldra1n",
                DefaultGalleryId = 1
            };
            Gallery AdminGallery = new Gallery
            {
                Id = 1,
                Name = "Default Gallery",
                Description = "Your Default Gallery",
                CreatorId = lukram.Id,
                StoredPictures = new List<GalleryStoredFile> { },
            };
            modelBuilder.Entity<Gallery>().HasData(AdminGallery);            
            //lukram.Galleries = new List<Gallery> { AdminGallery };
            modelBuilder.Entity<AppUser>().HasData(lukram);
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "a91913c7-a024-47d1-8f17-d388e7aeb211",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            });
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = "a91913c7-a024-47d1-8f17-d388e7aeb211",
                UserId = "fc7a69e4-c851-4eca-ba64-ac182cc4fc95"
            });
        }
    }
}