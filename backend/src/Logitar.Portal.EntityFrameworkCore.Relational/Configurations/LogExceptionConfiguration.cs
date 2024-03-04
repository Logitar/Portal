using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class LogExceptionConfiguration : IEntityTypeConfiguration<LogExceptionEntity>
{
  public void Configure(EntityTypeBuilder<LogExceptionEntity> builder)
  {
    builder.ToTable(nameof(PortalContext.LogExceptions));
    builder.HasKey(x => x.LogExceptionId);

    builder.HasIndex(x => x.Type);

    builder.Ignore(x => x.Data);

    builder.Property(x => x.Type).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DataSerialized).HasColumnName(nameof(LogExceptionEntity.Data));

    builder.HasOne(x => x.Log).WithMany(x => x.Exceptions).OnDelete(DeleteBehavior.Cascade);
  }
}
