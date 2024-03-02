using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class LogConfiguration : IEntityTypeConfiguration<LogEntity>
{
  public void Configure(EntityTypeBuilder<LogEntity> builder)
  {
    builder.ToTable(nameof(PortalContext.Logs));
    builder.HasKey(x => x.LogId);

    builder.HasIndex(x => x.UniqueId).IsUnique();
    builder.HasIndex(x => x.CorrelationId);
  }
}
