using Logitar.Portal.Infrastructure2.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure2.Configurations
{
  internal class EnumConfiguration<TEntity, TKey> where TEntity : EnumEntity<TKey>
  {
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
      builder.HasKey(x => x.Value);

      builder.HasIndex(x => x.Name).IsUnique();

      builder.Property(x => x.Name).HasMaxLength(256);
    }
  }
}
