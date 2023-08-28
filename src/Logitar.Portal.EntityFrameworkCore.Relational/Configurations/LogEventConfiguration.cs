using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class LogEventConfiguration : IEntityTypeConfiguration<LogEventEntity>
{
  public void Configure(EntityTypeBuilder<LogEventEntity> builder)
  {
    builder.ToTable(nameof(PortalContext.LogEvents));
    builder.HasKey(x => new { x.LogId, x.EventId });
  }
}
