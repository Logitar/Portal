using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class LogConfiguration : IEntityTypeConfiguration<Log>
  {
    public void Configure(EntityTypeBuilder<Log> builder)
    {
      builder.HasKey(x => x.Sid);
      builder.HasIndex(x => x.Id).IsUnique();

      builder.Ignore(x => x.Errors);
      builder.Ignore(x => x.Request);

      builder.Property(x => x.ActorId).HasDefaultValue(Guid.Empty);
      builder.Property(x => x.ErrorsSerialized).HasColumnName(nameof(Log.Errors)).HasColumnType("jsonb");
      builder.Property(x => x.HasErrors).HasDefaultValue(false);
      builder.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
      builder.Property(x => x.IpAddress).HasMaxLength(40);
      builder.Property(x => x.Method).HasMaxLength(10);
      builder.Property(x => x.RequestSerialized).HasColumnName(nameof(Log.Request)).HasColumnType("jsonb");
      builder.Property(x => x.StartedAt).HasDefaultValueSql("now()");
      builder.Property(x => x.Succeeded).HasDefaultValue(false);
      builder.Property(x => x.Url).HasMaxLength(2048);
    }
  }
}
