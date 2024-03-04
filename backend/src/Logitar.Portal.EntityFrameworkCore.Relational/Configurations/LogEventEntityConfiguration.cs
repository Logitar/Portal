using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class LogEventEntityConfiguration : IEntityTypeConfiguration<LogEventEntity>
{
  public void Configure(EntityTypeBuilder<LogEventEntity> builder)
  {
    builder.ToTable(nameof(PortalContext.LogEvents));
    builder.HasKey(x => x.EventId);

    builder.HasIndex(x => x.LogId);

    builder.HasOne(x => x.Log).WithMany(x => x.Events).OnDelete(DeleteBehavior.Cascade);
  }
}
