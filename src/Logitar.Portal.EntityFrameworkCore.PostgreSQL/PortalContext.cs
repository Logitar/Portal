using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL;

public class PortalContext : DbContext
{
  public PortalContext(DbContextOptions<PortalContext> options) : base(options)
  {
  }

  internal DbSet<DictionaryEntity> Dictionaries { get; private set; } = null!;
  internal DbSet<ExternalIdentifierEntity> ExternalIdentifiers { get; private set; } = null!;
  internal DbSet<MessageEntity> Messages { get; private set; } = null!;
  internal DbSet<RealmEntity> Realms { get; private set; } = null!;
  internal DbSet<SenderEntity> Senders { get; private set; } = null!;
  internal DbSet<SessionEntity> Sessions { get; private set; } = null!;
  internal DbSet<TemplateEntity> Templates { get; private set; } = null!;
  internal DbSet<BlacklistedTokenEntity> TokenBlacklist { get; private set; } = null!;
  internal DbSet<UserEntity> Users { get; private set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortalContext).Assembly);
  }
}
