using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class UserConfiguration : AggregateConfiguration<UserEntity>, IEntityTypeConfiguration<UserEntity>
  {
    public override void Configure(EntityTypeBuilder<UserEntity> builder)
    {
      base.Configure(builder);

      builder.HasKey(x => x.UserId);

      builder.HasIndex(x => x.Email);
      builder.HasIndex(x => x.EmailNormalized);
      builder.HasIndex(x => x.FirstName);
      builder.HasIndex(x => x.IsAccountConfirmed);
      builder.HasIndex(x => x.IsDisabled);
      builder.HasIndex(x => x.LastName);
      builder.HasIndex(x => x.MiddleName);
      builder.HasIndex(x => x.PhoneNumber);
      builder.HasIndex(x => x.Username);
      builder.HasIndex(x => new { x.RealmId, x.UsernameNormalized }).IsUnique();

      builder.Property(x => x.DisabledBy).HasMaxLength(256);
      builder.Property(x => x.Email).HasMaxLength(256);
      builder.Property(x => x.EmailConfirmedBy).HasMaxLength(256);
      builder.Property(x => x.EmailNormalized).HasMaxLength(256);
      builder.Property(x => x.FirstName).HasMaxLength(128);
      builder.Property(x => x.FullName).HasMaxLength(512);
      builder.Property(x => x.LastName).HasMaxLength(128);
      builder.Property(x => x.Locale).HasMaxLength(16);
      builder.Property(x => x.MiddleName).HasMaxLength(128);
      builder.Property(x => x.PhoneNumberConfirmedBy).HasMaxLength(256);
      builder.Property(x => x.Picture).HasMaxLength(2048);
      builder.Property(x => x.Username).HasMaxLength(256);
      builder.Property(x => x.UsernameNormalized).HasMaxLength(256);
    }
  }
}
