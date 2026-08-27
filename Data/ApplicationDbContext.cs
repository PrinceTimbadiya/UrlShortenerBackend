using Microsoft.EntityFrameworkCore;
using UrlShortenerBackend.Models.Entities;

namespace UrlShortenerBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserMaster> UserMaster { get; set; }

        public DbSet<LoginToken> LoginToken { get; set; }

        public DbSet<UrlMaster> UrlMaster { get; set; }

        public DbSet<CredentialMaster> CredentialMaster { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserMaster → UrlMaster
            modelBuilder.Entity<UrlMaster>()
                .HasOne(x => x.UserMaster)
                .WithMany(x => x.UrlMasters)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ShortCode must be unique
            modelBuilder.Entity<UrlMaster>()
                .HasIndex(x => x.ShortCode)
                .IsUnique();

            // UserMaster → CredentialMaster
            modelBuilder.Entity<CredentialMaster>()
                .HasOne(x => x.UserMaster)
                .WithMany(x => x.CredentialMasters)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApiKey must be unique
            modelBuilder.Entity<CredentialMaster>()
                .HasIndex(x => x.ApiKey)
                .IsUnique();
        }
    }
}