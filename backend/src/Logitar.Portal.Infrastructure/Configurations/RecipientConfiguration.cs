using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class RecipientConfiguration : IEntityTypeConfiguration<RecipientEntity>
  {
    public void Configure(EntityTypeBuilder<RecipientEntity> builder)
    {
      builder.HasKey(x => x.RecipientId);

      builder.HasOne(x => x.Message).WithMany(x => x.Recipients).OnDelete(DeleteBehavior.Cascade);

      builder.HasIndex(x => x.Id).IsUnique();

      builder.Property(x => x.Address).HasMaxLength(256);
      builder.Property(x => x.DisplayName).HasMaxLength(512);

      builder.Property(x => x.UserId).HasMaxLength(256);
      builder.Property(x => x.Username).HasMaxLength(256);
      builder.Property(x => x.UserLocale).HasMaxLength(16);
    }
  }
}
