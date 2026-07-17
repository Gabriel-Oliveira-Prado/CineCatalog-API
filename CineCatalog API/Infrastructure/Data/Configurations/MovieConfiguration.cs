using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CineCatalog_API.Domain.Entities;

namespace CineCatalog_API.Infrastructure.Data.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(m => m.Title); // index for titles searching

            builder.HasIndex(m => m.TmdbId)
                .IsUnique()
                .HasFilter("[TmdbId] IS NOT NULL");

            builder.Property(m => m.Description)
                .HasMaxLength(1000);

            builder.Property(m => m.Synopsis)
                .HasMaxLength(4000);

            builder.Property(m => m.Director)
                .HasMaxLength(150);

            builder.Property(m => m.Cast)
                .HasMaxLength(2000);

            builder.Property(m => m.Rating)
                .HasMaxLength(20);

            builder.Property(m => m.ImageUrl)
                .HasMaxLength(1000);

            builder.Property(m => m.TrailerUrl)
                .HasMaxLength(1000);

            // Configure many-to-many relationship with Genre
            builder.HasMany(m => m.Genres)
                .WithMany(g => g.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieGenre",
                    j => j.HasOne<Genre>().WithMany().HasForeignKey("GenreId"),
                    j => j.HasOne<Movie>().WithMany().HasForeignKey("MovieId"),
                    je =>
                    {
                        je.HasKey("MovieId", "GenreId");
                    });
        }
    }
}
