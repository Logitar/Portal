using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

public class PortalContext : DbContext
{
  public PortalContext(DbContextOptions<PortalContext> options) : base(options)
  {
  }

  internal DbSet<DictionaryEntity> Dictionaries { get; private set; }
  internal DbSet<RealmEntity> Realms { get; private set; }
  internal DbSet<SenderEntity> Senders { get; private set; }
  internal DbSet<TemplateEntity> Templates { get; private set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
