using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CineCatalog_API.Domain.Entities;

namespace CineCatalog_API.Infrastructure.Data.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Comment)
                .HasMaxLength(2000);

            builder.Property(r => r.Rating)
                .IsRequired();

            // Establish relationships
            builder.HasOne(r => r.Movie)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Enforce business rule: A user can only review a movie once
            builder.HasIndex(r => new { r.UserId, r.MovieId })
                .IsUnique();
        }
    }
}
