using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Satur8.WPF.Domain.Models;

namespace Satur8.WPF.Infrastructure.EntityTypeConfiguration
{
    public class PresetConfiguration : IEntityTypeConfiguration<Preset>
    {
        public void Configure(EntityTypeBuilder<Preset> builder)
        {
            builder.HasKey(e => e.PresetId).HasName("presets_pkey");

            builder.ToTable("presets");

            builder.Property(e => e.PresetId).HasColumnName("preset_id");
            builder.Property(e => e.CategoryId).HasColumnName("category_id");
            builder.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            builder.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            builder.Property(e => e.ParametersJson)
                .HasColumnType("json")
                .HasColumnName("parameters_json");
            builder.Property(e => e.UserId).HasColumnName("user_id");

            builder.HasOne(d => d.Category).WithMany(p => p.Presets)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("presets_category_id_fkey");

            builder.HasOne(d => d.User).WithMany(p => p.Presets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("presets_user_id_fkey");
        }
    }
}
