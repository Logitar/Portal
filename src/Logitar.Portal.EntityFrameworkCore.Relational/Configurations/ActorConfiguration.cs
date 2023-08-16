using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class ActorConfiguration : IEntityTypeConfiguration<ActorEntity>
{
  public void Configure(EntityTypeBuilder<ActorEntity> builder)
  {
    builder.ToTable(nameof(PortalContext.Actors));
    builder.HasKey(x => x.ActorId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.Type);
    builder.HasIndex(x => x.IsDeleted);
    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.Id).HasMaxLength(byte.MaxValue).IsRequired();
    builder.Property(x => x.Type).HasMaxLength(byte.MaxValue)
      .HasConversion(new EnumToStringConverter<ActorType>());
    builder.Property(x => x.DisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.EmailAddress).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.PictureUrl).HasMaxLength(2048);
  }
}
