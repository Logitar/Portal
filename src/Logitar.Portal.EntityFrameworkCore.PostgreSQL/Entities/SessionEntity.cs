using Logitar.Portal.Core.Sessions.Events;
using System.Text.Json;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;

internal class SessionEntity : AggregateEntity, ICustomAttributes
{
  public SessionEntity(SessionCreated e, UserEntity user) : base(e, ActorEntity.From(user))
  {
    User = user;
    UserId = user.UserId;

    Key = e.Key?.ToString();

    IsActive = true;

    Apply(e);
  }

  private SessionEntity() : base()
  {
  }

  public int SessionId { get; private set; }

  public UserEntity? User { get; private set; }
  public int UserId { get; private set; }

  public string? Key { get; private set; }
  public bool IsPersistent
  {
    get => Key != null;
    private set { }
  }

  public Guid? SignedOutById { get; private set; }
  public string? SignedOutBy { get; private set; }
  public DateTime? SignedOutOn { get; private set; }
  public bool IsActive { get; private set; }

  public string? IpAddress { get; private set; }
  public string? AdditionalInformation { get; private set; }

  public string? CustomAttributes { get; private set; }

  public void Refresh(SessionRefreshed e)
  {
    if (User == null)
    {
      throw new InvalidOperationException($"The {nameof(User)} is required.");
    }

    Update(e, ActorEntity.From(User));

    Key = e.Key.ToString();

    Apply(e);
  }

  public override void SetActor(Guid id, ActorEntity actor)
  {
    base.SetActor(id, actor);

    if (SignedOutById == id)
    {
      SignedOutBy = actor.Serialize();
    }
  }

  public void SignOut(SessionSignedOut e, ActorEntity actor)
  {
    SignedOutById = e.ActorId.ToGuid();
    SignedOutBy = actor.Serialize();
    SignedOutOn = e.OccurredOn;
    IsActive = false;
  }

  private void Apply(SessionSaved e)
  {
    IpAddress = e.IpAddress;
    AdditionalInformation = e.AdditionalInformation;

    CustomAttributes = e.CustomAttributes.Any() ? JsonSerializer.Serialize(e.CustomAttributes) : null;
  }
}
