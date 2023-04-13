using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Configurations;

internal class ActivityConfiguration : IEntityTypeConfiguration<ActivityEntity>
{
  public void Configure(EntityTypeBuilder<ActivityEntity> builder)
  {
    builder.ToTable(nameof(PortalContext.Activities), Schemas.Logging);

    builder.HasKey(x => x.ActivityId);

    builder.HasIndex(x => x.Id).IsUnique();

    builder.Property(x => x.Type).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Data).HasColumnType("jsonb");
    builder.Property(x => x.Level).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Errors).HasColumnType("jsonb");

    builder.HasOne(x => x.Log).WithMany(x => x.Activities).OnDelete(DeleteBehavior.Cascade);
  }
}
