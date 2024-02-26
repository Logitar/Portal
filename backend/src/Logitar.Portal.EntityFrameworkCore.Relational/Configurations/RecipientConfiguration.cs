using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational.Configurations;
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

    builder.HasIndex(x => x.Type);
    builder.HasIndex(x => x.Address);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.UserId);

    builder.Property(x => x.Type).HasMaxLength(3).HasConversion(new EnumToStringConverter<RecipientType>());
    builder.Property(x => x.Address).HasMaxLength(EmailUnit.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
    builder.Property(x => x.UserUniqueName).HasMaxLength(UniqueNameUnit.MaximumLength);
    builder.Property(x => x.UserEmailAddress).HasMaxLength(EmailUnit.MaximumLength);
    builder.Property(x => x.UserFullName).HasMaxLength(UserConfiguration.FullNameMaximumLength);
    builder.Property(x => x.UserPicture).HasMaxLength(UrlUnit.MaximumLength);

    builder.HasOne(x => x.Message).WithMany(x => x.Recipients).OnDelete(DeleteBehavior.Cascade);
  }
}
