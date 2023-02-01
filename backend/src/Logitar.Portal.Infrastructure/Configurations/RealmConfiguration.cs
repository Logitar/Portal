﻿using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class RealmConfiguration : AggregateConfiguration<RealmEntity>, IEntityTypeConfiguration<RealmEntity>
  {
    public override void Configure(EntityTypeBuilder<RealmEntity> builder)
    {
      base.Configure(builder);

      builder.HasKey(x => x.RealmId);

      builder.HasIndex(x => x.AliasNormalized).IsUnique();

      builder.Property(x => x.Alias).HasMaxLength(256);
      builder.Property(x => x.AliasNormalized).HasMaxLength(256);
      builder.Property(x => x.DisplayName).HasMaxLength(256);
      builder.Property(x => x.DefaultLocale).HasMaxLength(16);
      builder.Property(x => x.Url).HasMaxLength(2048);
      builder.Property(x => x.UsernameSettings).HasColumnType("jsonb");
      builder.Property(x => x.PasswordSettings).HasColumnType("jsonb");
      builder.Property(x => x.GoogleClientId).HasMaxLength(256);
    }
  }
}
