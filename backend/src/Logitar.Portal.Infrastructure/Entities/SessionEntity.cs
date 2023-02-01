using Logitar.Portal.Domain.Sessions.Events;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class SessionEntity : AggregateEntity
  {
    public SessionEntity(SessionCreatedEvent @event, UserEntity user) : base(@event)
    {
      User = user;
      UserId = user.UserId;

      KeyHash = @event.KeyHash;

      IpAddress = @event.IpAddress;
      AdditionalInformation = @event.AdditionalInformation;
    }
    private SessionEntity() : base()
    {
    }

    public int SessionId { get; private set; }

    public UserEntity? User { get; private set; }
    public int UserId { get; private set; }

    public string? KeyHash { get; private set; }
    public bool IsPersistent
    {
      get => KeyHash != null;
      private set { }
    }

    public string? SignedOutBy { get; private set; }
    public DateTime? SignedOutOn { get; private set; }
    public bool IsActive
    {
      get => SignedOutBy == null && !SignedOutOn.HasValue;
      private set { }
    }

    public string? IpAddress { get; private set; }
    public string? AdditionalInformation { get; private set; }

    public void Renew(SessionRenewedEvent @event)
    {
      Update(@event);

      KeyHash = @event.KeyHash;

      IpAddress = @event.IpAddress;
      AdditionalInformation = @event.AdditionalInformation;
    }
    public void SignOut(SessionSignedOutEvent @event)
    {
      SignedOutBy = @event.UserId.Value;
      SignedOutOn = @event.OccurredOn;
    }
  }
}
