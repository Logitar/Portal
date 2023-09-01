using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class UserIdentifierConfiguration : IdentifierConfiguration<UserIdentifierEntity>, IEntityTypeConfiguration<UserIdentifierEntity>
{
  public override void Configure(EntityTypeBuilder<UserIdentifierEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(PortalContext.UserIdentifiers));
    builder.HasKey(x => x.UserIdentifierId);
  }
}
