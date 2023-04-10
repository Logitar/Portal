using Logitar.Portal.v2.Core.Sessions.Events;
using System.Text.Json;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;

internal class SessionEntity : AggregateEntity, ICustomAttributes
{
  public SessionEntity(SessionCreated e, UserEntity user, ActorEntity actor)
  {
    User = user;
    UserId = user.UserId;

    Key = e.Key?.ToString();

    IsActive = true;

    CustomAttributes = e.CustomAttributes.Any() ? JsonSerializer.Serialize(e.CustomAttributes) : null;
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

  public string? CustomAttributes { get; private set; }

  public override void SetActor(Guid id, ActorEntity actor)
  {
    base.SetActor(id, actor);

    if (SignedOutById == id)
    {
      SignedOutBy = actor.Serialize();
    }
  }
}
