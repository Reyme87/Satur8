using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Satur8.Domain.Models;

namespace Satur8.Persistence.EntityTypeConfiguration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(e => e.CategoryId).HasName("categories_pkey");

            builder.ToTable("categories");

            builder.HasIndex(e => e.Name, "validate_categories_name").IsUnique();

            builder.Property(e => e.CategoryId).HasColumnName("category_id");
            builder.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        }
    }
}
