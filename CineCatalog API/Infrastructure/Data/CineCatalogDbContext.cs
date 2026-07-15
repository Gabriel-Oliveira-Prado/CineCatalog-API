using Microsoft.EntityFrameworkCore;
using CineCatalog_API.Domain.Entities;

namespace CineCatalog_API.Infrastructure.Data
{
    public class CineCatalogDbContext : DbContext
    {
        public CineCatalogDbContext(DbContextOptions<CineCatalogDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Favorite> Favorites => Set<Favorite>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CineCatalogDbContext).Assembly);
        }
    }
}
