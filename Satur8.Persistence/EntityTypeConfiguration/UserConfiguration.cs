using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Satur8.WPF.Domain.Models;

namespace Satur8.WPF.Infrastructure.EntityTypeConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.UserId).HasName("users_pkey");

            builder.ToTable("users");

            builder.Property(e => e.UserId).HasColumnName("user_id");
            builder.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            builder.Property(e => e.PasswordHash)
                .HasMaxLength(50)
                .HasColumnName("password_hash");
        }
    }
}
