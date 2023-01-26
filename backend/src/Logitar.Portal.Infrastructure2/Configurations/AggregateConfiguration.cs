using Logitar.Portal.Infrastructure2.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure2.Configurations
{
  internal abstract class AggregateConfiguration<T> where T : AggregateEntity
  {
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
      builder.HasIndex(x => x.AggregateId).IsUnique();

      builder.Property(x => x.AggregateId).HasMaxLength(256);
      builder.Property(x => x.CreatedBy).HasMaxLength(256);
      builder.Property(x => x.UpdatedBy).HasMaxLength(256);
    }
  }
}
