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

      builder.HasOne(x => x.Realm).WithMany(x => x.Users).OnDelete(DeleteBehavior.Restrict);

      builder.HasIndex(x => x.Username);
      builder.HasIndex(x => x.PasswordChangedOn);
      builder.HasIndex(x => x.Email);
      builder.HasIndex(x => x.EmailNormalized);
      builder.HasIndex(x => x.EmailConfirmedById);
      builder.HasIndex(x => x.PhoneNumber);
      builder.HasIndex(x => x.PhoneNumberConfirmedById);
      builder.HasIndex(x => x.IsAccountConfirmed);
      builder.HasIndex(x => x.FirstName);
      builder.HasIndex(x => x.MiddleName);
      builder.HasIndex(x => x.LastName);
      builder.HasIndex(x => x.SignedInOn);
      builder.HasIndex(x => x.DisabledById);
      builder.HasIndex(x => x.IsDisabled);
      builder.HasIndex(x => new { x.RealmId, x.UsernameNormalized }).IsUnique();

      builder.Property(x => x.Username).HasMaxLength(255);
      builder.Property(x => x.UsernameNormalized).HasMaxLength(255);
      builder.Property(x => x.Email).HasMaxLength(255);
      builder.Property(x => x.EmailNormalized).HasMaxLength(255);
      builder.Property(x => x.EmailConfirmedById).HasMaxLength(255);
      builder.Property(x => x.EmailConfirmedBy).HasColumnType("jsonb");
      builder.Property(x => x.PhoneNumberConfirmedById).HasMaxLength(255);
      builder.Property(x => x.PhoneNumberConfirmedBy).HasColumnType("jsonb");
      builder.Property(x => x.FirstName).HasMaxLength(127);
      builder.Property(x => x.MiddleName).HasMaxLength(127);
      builder.Property(x => x.LastName).HasMaxLength(127);
      builder.Property(x => x.FullName).HasMaxLength(383);
      builder.Property(x => x.Locale).HasMaxLength(16);
      builder.Property(x => x.Picture).HasMaxLength(2048);
      builder.Property(x => x.DisabledById).HasMaxLength(255);
      builder.Property(x => x.DisabledBy).HasColumnType("jsonb");
    }
  }
}
