﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portal.Core.Realms;

namespace Portal.Infrastructure.Configurations
{
  internal class PasswordRecoverySenderConfiguration : IEntityTypeConfiguration<PasswordRecoverySender>
  {
    public void Configure(EntityTypeBuilder<PasswordRecoverySender> builder)
    {
      builder.ToTable("RealmPasswordRecoverySenders");

      builder.HasKey(x => x.RealmSid);

      builder.HasOne(x => x.Realm).WithOne(x => x.PasswordRecoverySenderRelation)
        .HasForeignKey<PasswordRecoverySender>(x => x.RealmSid);
    }
  }
}
