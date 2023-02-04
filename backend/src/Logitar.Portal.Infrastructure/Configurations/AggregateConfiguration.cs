using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal abstract class AggregateConfiguration<T> where T : AggregateEntity
  {
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
      builder.HasIndex(x => x.AggregateId).IsUnique();
      builder.HasIndex(x => x.CreatedOn);
      builder.HasIndex(x => x.UpdatedOn);

      builder.Property(x => x.AggregateId).HasMaxLength(256).IsRequired();
      builder.Property(x => x.CreatedBy).HasMaxLength(256);
      builder.Property(x => x.UpdatedBy).HasMaxLength(256);
    }
  }
}
