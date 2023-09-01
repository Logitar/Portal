using Logitar.EventSourcing;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class IdentifierConfiguration<T> where T : IdentifierEntity
{
  public virtual void Configure(EntityTypeBuilder<T> builder)
  {
    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => new { x.TenantId, x.Key, x.Value }).IsUnique();

    builder.Property(x => x.TenantId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Key).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Value).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.ValueNormalized).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);
  }
}
