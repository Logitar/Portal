namespace Logitar.Portal.Infrastructure.Entities
{
  internal class BlacklistedJwtEntity
  {
    public BlacklistedJwtEntity(Guid id, DateTime? expiresOn = null)
    {
      Id = id;
      ExpiresOn = expiresOn;
    }
    private BlacklistedJwtEntity()
    {
    }

    public long BlacklistedJwtId { get; private set; }
    public Guid Id { get; private set; }
    public DateTime? ExpiresOn { get; private set; }
  }
}
