using Logitar.Portal.Domain.Sessions.Events;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class SessionEntity : AggregateEntity
  {
    public SessionEntity(SessionCreatedEvent @event, UserEntity user) : base(@event, new Actor(user))
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

    public string? SignedOutById { get; private set; }
    public string? SignedOutBy { get; private set; }
    public DateTime? SignedOutOn { get; private set; }
    public bool IsActive
    {
      get => SignedOutById == null && !SignedOutOn.HasValue;
      private set { }
    }

    public string? IpAddress { get; private set; }
    public string? AdditionalInformation { get; private set; }

    public override void UpdateActors(string id, Actor actor)
    {
      base.UpdateActors(id, actor);

      if (SignedOutById == id)
      {
        SignedOutBy = actor.Serialize();
      }
    }

    public void Renew(SessionRenewedEvent @event, Actor actor)
    {
      Update(@event, actor);

      KeyHash = @event.KeyHash;

      IpAddress = @event.IpAddress;
      AdditionalInformation = @event.AdditionalInformation;
    }
    public void SignOut(SessionSignedOutEvent @event, Actor actor)
    {
      SignedOutById = @event.ActorId.Value;
      SignedOutBy = actor.Serialize();
      SignedOutOn = @event.OccurredOn;
    }
  }
}
