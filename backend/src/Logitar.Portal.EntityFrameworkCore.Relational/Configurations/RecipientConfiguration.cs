﻿using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
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
    builder.HasIndex(x => x.PhoneNumber);
    builder.HasIndex(x => x.UserId);

    builder.Property(x => x.Type).HasMaxLength(3).HasConversion(new EnumToStringConverter<RecipientType>());
    builder.Property(x => x.Address).HasMaxLength(Email.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.PhoneNumber).HasMaxLength(Phone.NumberMaximumLength);
    builder.Property(x => x.UserUniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UserEmailAddress).HasMaxLength(Email.MaximumLength);
    builder.Property(x => x.UserFullName).HasMaxLength(UserConfiguration.FullNameMaximumLength);
    builder.Property(x => x.UserPicture).HasMaxLength(Url.MaximumLength);

    builder.HasOne(x => x.Message).WithMany(x => x.Recipients).OnDelete(DeleteBehavior.Cascade);
  }
}
