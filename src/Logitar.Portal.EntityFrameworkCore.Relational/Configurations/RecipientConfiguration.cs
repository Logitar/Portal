using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class RecipientConfiguration : IEntityTypeConfiguration<RecipientEntity>
{
  public void Configure(EntityTypeBuilder<RecipientEntity> builder)
  {
    builder.ToTable(nameof(PortalContext.Recipients));
    builder.HasKey(x => x.RecipientId);

    builder.Ignore(x => x.UserSummary);

    builder.Property(x => x.Type).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<RecipientType>());
    builder.Property(x => x.Address).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.UserSummarySerialized).HasColumnName(nameof(RecipientEntity.UserSummary));

    builder.HasOne(x => x.User).WithMany(x => x.Recipients).OnDelete(DeleteBehavior.SetNull);
  }
}
