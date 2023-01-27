using Logitar.Portal.Core.Sessions.Events;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Sessions
{
  public class Session : AggregateRoot
  {
    public Session(User user, string? keyHash = null, string? ipAddress = null, string? additionalInformation = null)
    {
      ApplyChange(new SessionCreatedEvent
      {
        KeyHash = keyHash,
        IpAddress = ipAddress,
        AdditionalInformation = additionalInformation
      }, user.Id);
    }
    private Session()
    {
    }

    public AggregateId UserId { get; private set; }

    public string? KeyHash { get; private set; }

    public bool IsActive { get; private set; } = true;

    public string? IpAddress { get; private set; }
    public string? AdditionalInformation { get; private set; }

    public void Delete(AggregateId userId) => ApplyChange(new SessionDeletedEvent(), userId);
    public void Renew(string keyHash, string? ipAddress = null, string? additionalInformation = null)
    {
      if (!IsActive)
      {
        throw new SessionSignedOutException(this);
      }

      ApplyChange(new SessionRenewedEvent
      {
        KeyHash = keyHash,
        IpAddress = ipAddress,
        AdditionalInformation = additionalInformation
      }, UserId);
    }
    public void SignOut(AggregateId? userId = null)
    {
      if (!IsActive)
      {
        throw new SessionSignedOutException(this);
      }

      ApplyChange(new SessionSignedOutEvent(), userId ?? UserId);
    }

    protected virtual void Apply(SessionCreatedEvent @event)
    {
      UserId = @event.UserId;

      KeyHash = @event.KeyHash;

      IpAddress = @event.IpAddress;
      AdditionalInformation = @event.AdditionalInformation;
    }
    protected virtual void Apply(SessionSignedOutEvent @event)
    {
      IsActive = false;
    }
  }
}
