using Microsoft.EntityFrameworkCore;
using ServarrAuthAPI.Database.Models;

namespace ServarrAuthAPI.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TraktEntity> TraktEntities { get; set; }

        public DbSet<SpotifyEntity> SpotifyEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TraktEntity>(builder =>
            {
                builder.HasKey(k => k.Id);
                builder.HasIndex(k => k.State);

                builder.Property(x => x.State).IsRequired();
                builder.Property(x => x.Target).IsRequired().HasMaxLength(255);
                builder.Property(x => x.CreatedAt).IsRequired();
            });
        }
    }
}
