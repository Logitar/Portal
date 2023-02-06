using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal abstract class EnumConfiguration<TEntity, TKey> where TEntity : EnumEntity<TKey> where TKey : new()
  {
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
      builder.HasKey(x => x.Value);

      builder.HasIndex(x => x.Name).IsUnique();

      builder.Property(x => x.Value).ValueGeneratedNever();
      builder.Property(x => x.Name).HasMaxLength(255);
    }
  }
}
