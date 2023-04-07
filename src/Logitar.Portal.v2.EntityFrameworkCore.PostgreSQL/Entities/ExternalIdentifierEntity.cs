using Logitar.Portal.v2.Core.Users.Events;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;

internal class ExternalIdentifierEntity
{
  public ExternalIdentifierEntity(ExternalIdentifierSet e, UserEntity user, ActorEntity actor)
  {
    if (e.Value == null)
    {
      throw new ArgumentException($"The {nameof(e.Value)} is required.", nameof(e));
    }
    if (user.RealmId.HasValue && user.Realm == null)
    {
      throw new ArgumentException($"The {nameof(user.Realm)} is required.", nameof(user));
    }

    Realm = user.Realm;
    RealmId = user.Realm?.RealmId;

    User = user;
    UserId = user.UserId;

    Key = e.Key;
    Value = e.Value;

    CreatedById = e.ActorId.ToGuid();
    CreatedBy = actor.Serialize();
    CreatedOn = e.OccurredOn;
  }

  private ExternalIdentifierEntity()
  {
  }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }

  public UserEntity? User { get; private set; }
  public int UserId { get; private set; }

  public string Key { get; private set; } = string.Empty;
  public string Value { get; private set; } = string.Empty;

  public Guid CreatedById { get; private set; }
  public string CreatedBy { get; private set; } = string.Empty;
  public DateTime CreatedOn { get; private set; }

  public Guid? UpdatedById { get; private set; }
  public string? UpdatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }

  public void Update(ExternalIdentifierSet e, ActorEntity actor)
  {
    Value = e.Value ?? throw new ArgumentException($"The {nameof(e.Value)} is required.", nameof(e));

    UpdatedById = e.ActorId.ToGuid();
    UpdatedBy = actor.Serialize();
    UpdatedOn = e.OccurredOn;
  }
}
