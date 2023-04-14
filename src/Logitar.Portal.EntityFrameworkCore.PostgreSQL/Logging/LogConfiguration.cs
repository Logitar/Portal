using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Logging;

internal class LogConfiguration : IEntityTypeConfiguration<LogEntity>
{
  public void Configure(EntityTypeBuilder<LogEntity> builder)
  {
    builder.ToTable(nameof(PortalContext.Logs), Schemas.Logging);

    builder.HasKey(x => x.LogId);

    builder.HasIndex(x => x.Id).IsUnique();

    builder.Property(x => x.CorrelationId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Method).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Destination).HasMaxLength(ushort.MaxValue);
    builder.Property(x => x.Source).HasMaxLength(ushort.MaxValue);
    builder.Property(x => x.OperationType).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.OperationName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Level).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Errors).HasColumnType("jsonb");
  }
}
