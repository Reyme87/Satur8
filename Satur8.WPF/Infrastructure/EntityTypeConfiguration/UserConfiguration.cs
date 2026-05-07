using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Satur8.WPF.Domain.Models;

namespace Satur8.WPF.Infrastructure.EntityTypeConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {

        }
    }
}
