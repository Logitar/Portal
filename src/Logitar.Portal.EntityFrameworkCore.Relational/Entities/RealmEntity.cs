namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record RealmEntity : AggregateEntity
{
  private RealmEntity() : base()
  {
  }

  public int RealmId { get; private set; }
}
