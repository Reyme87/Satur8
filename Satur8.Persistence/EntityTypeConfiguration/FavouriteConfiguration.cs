using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Satur8.Domain.Models;

namespace Satur8.Persistence.EntityTypeConfiguration
{
    public class FavouriteConfiguration : IEntityTypeConfiguration<Favourite>
    {
        public void Configure(EntityTypeBuilder<Favourite> builder)
        {
            builder.HasKey(e => e.FavouritesId).HasName("favourites_pkey");

            builder.ToTable("favourites");

            builder.Property(e => e.FavouritesId).HasColumnName("favourites_id");
            builder.Property(e => e.PresetId).HasColumnName("preset_id");
            builder.Property(e => e.UserId).HasColumnName("user_id");

            builder.HasOne(d => d.Preset).WithMany(p => p.Favourites)
                .HasForeignKey(d => d.PresetId)
                .HasConstraintName("favourites_preset_id_fkey")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.User).WithMany(p => p.Favourites)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("favourites_user_id_fkey")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
