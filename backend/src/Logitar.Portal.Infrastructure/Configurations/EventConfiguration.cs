using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class EventConfiguration : IEntityTypeConfiguration<EventEntity>
  {
    public void Configure(EntityTypeBuilder<EventEntity> builder)
    {
      builder.HasKey(x => x.EventId);

      builder.HasIndex(x => x.Version);
      builder.HasIndex(x => new { x.AggregateType, x.AggregateId });

      builder.Property(x => x.UserId).HasMaxLength(256);
      builder.Property(x => x.AggregateType).HasMaxLength(256);
      builder.Property(x => x.AggregateId).HasMaxLength(256);
      builder.Property(x => x.EventType).HasMaxLength(256);
      builder.Property(x => x.EventData).HasColumnType("jsonb");
    }
  }
}
