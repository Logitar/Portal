using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Logitar.Portal.Core;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal abstract class AggregateConfiguration<T> : IEntityTypeConfiguration<T>
    where T : Aggregate
  {
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
      builder.HasKey(x => x.Sid);
      builder.HasIndex(x => x.Id).IsUnique();

      builder.Ignore(x => x.DeletedAt);
      builder.Ignore(x => x.DeletedById);

      builder.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
      builder.Property(x => x.CreatedById).HasDefaultValue(Guid.Empty);
      builder.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
      builder.Property(x => x.Version).HasDefaultValue(0);
    }
  }
}
