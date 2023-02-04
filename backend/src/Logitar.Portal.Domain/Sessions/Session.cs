using Logitar.Portal.Domain.Sessions.Events;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Domain.Sessions
{
  public class Session : AggregateRoot
  {
    public Session(User user, string? keyHash = null, string? ipAddress = null, string? additionalInformation = null) : base()
    {
      ApplyChange(new SessionCreatedEvent
      {
        KeyHash = keyHash,
        IpAddress = ipAddress?.CleanTrim(),
        AdditionalInformation = additionalInformation?.CleanTrim()
      }, user.Id);
    }
    private Session() : base()
    {
    }

    public AggregateId UserId { get; private set; }

    public string? KeyHash { get; private set; }
    public bool IsPersistent => KeyHash != null;

    public bool IsActive { get; private set; }

    public string? IpAddress { get; private set; }
    public string? AdditionalInformation { get; private set; }

    public void Delete(AggregateId actorId) => ApplyChange(new SessionDeletedEvent(), actorId);
    public void Renew(string? keyHash = null, string? ipAddress = null, string? additionalInformation = null)
    {
      if (!IsActive)
      {
        throw new SessionIsNotActiveException(this);
      }

      ApplyChange(new SessionRenewedEvent
      {
        KeyHash = keyHash,
        IpAddress = ipAddress?.CleanTrim(),
        AdditionalInformation = additionalInformation?.CleanTrim()
      }, UserId);
    }
    public void SignOut(AggregateId actorId)
    {
      if (!IsActive)
      {
        throw new SessionAlreadySignedOutException(this);
      }

      ApplyChange(new SessionSignedOutEvent(), actorId);
    }

    protected virtual void Apply(SessionCreatedEvent @event)
    {
      UserId = @event.ActorId;

      KeyHash = @event.KeyHash;

      IsActive = true;

      IpAddress = @event.IpAddress;
      AdditionalInformation = @event.AdditionalInformation;
    }
    protected virtual void Apply(SessionDeletedEvent @event)
    {
      Delete();
    }
    protected virtual void Apply(SessionRenewedEvent @event)
    {
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
