using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class LogConfiguration : IEntityTypeConfiguration<LogEntity>
  {
    public void Configure(EntityTypeBuilder<LogEntity> builder)
    {
      builder.HasKey(x => x.LogId);

      builder.HasIndex(x => x.Id).IsUnique();

      builder.Property(x => x.IpAddress).HasMaxLength(64);
      builder.Property(x => x.Method).HasMaxLength(16);
      builder.Property(x => x.Url).HasMaxLength(2048);
      builder.Property(x => x.ActorId).HasMaxLength(255);
      builder.Property(x => x.Actor).HasColumnType("jsonb");
      builder.Property(x => x.ApiKeyId).HasMaxLength(255);
      builder.Property(x => x.UserId).HasMaxLength(255);
      builder.Property(x => x.SessionId).HasMaxLength(255);
      builder.Property(x => x.ActivityType).HasMaxLength(255);
      builder.Property(x => x.ActivityData).HasColumnType("jsonb");
      builder.Property(x => x.Errors).HasColumnType("jsonb");
      builder.Property(x => x.Level).HasMaxLength(16);
    }
  }
}
