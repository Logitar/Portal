﻿using Logitar.EventSourcing;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class SessionConfiguration : AggregateConfiguration<SessionEntity>, IEntityTypeConfiguration<SessionEntity>
{
  public override void Configure(EntityTypeBuilder<SessionEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(PortalContext.Sessions));
    builder.HasKey(x => x.SessionId);

    builder.HasIndex(x => x.IsPersistent);
    builder.HasIndex(x => x.IsActive);
    builder.HasIndex(x => x.SignedOutOn);

    builder.Ignore(x => x.CustomAttributes);

    builder.Property(x => x.Secret).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.SignedOutBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.CustomAttributesSerialized).HasColumnName(nameof(SessionEntity.CustomAttributes));

    builder.HasOne(x => x.User).WithMany(x => x.Sessions).OnDelete(DeleteBehavior.Restrict);
  }
}
