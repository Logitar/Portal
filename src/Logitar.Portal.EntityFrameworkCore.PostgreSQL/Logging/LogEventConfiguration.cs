using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Logging;

internal class LogEventConfiguration : IEntityTypeConfiguration<LogEventEntity>
{
  public void Configure(EntityTypeBuilder<LogEventEntity> builder)
  {
    builder.ToTable("Events", Schemas.Logging);

    builder.HasKey(x => x.EventId);

    builder.Property(x => x.EventId).ValueGeneratedNever();

    builder.HasOne(x => x.Log).WithMany(x => x.Events).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Activity).WithMany(x => x.Events).OnDelete(DeleteBehavior.Cascade);
  }
}
