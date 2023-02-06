using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class LogEventConfiguration : IEntityTypeConfiguration<LogEventEntity>
  {
    public void Configure(EntityTypeBuilder<LogEventEntity> builder)
    {
      builder.HasKey(x => x.EventId);

      builder.HasOne(x => x.Event).WithOne(x => x.Log);
      builder.HasOne(x => x.Log).WithMany(x => x.Events);
    }
  }
}
