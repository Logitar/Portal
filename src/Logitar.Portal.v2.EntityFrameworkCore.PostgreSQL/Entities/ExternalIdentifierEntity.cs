using Logitar.Portal.v2.Core.Users.Events;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;

internal class ExternalIdentifierEntity
{
  public ExternalIdentifierEntity(ExternalIdentifierSet e, UserEntity user, ActorEntity actor)
  {
    Id = Guid.NewGuid();

    Realm = user.Realm;
    RealmId = user.Realm?.RealmId ?? throw new ArgumentException($"The {nameof(user.Realm)} is required.", nameof(user)); ;

    User = user;
    UserId = user.UserId;

    Key = e.Key;
    Value = e.Value ?? throw new ArgumentException($"The {nameof(e.Value)} is required.", nameof(e)); ;

    CreatedById = e.ActorId.ToGuid();
    CreatedBy = actor.Serialize();
    CreatedOn = e.OccurredOn;
  }

  private ExternalIdentifierEntity()
  {
  }

  public int ExternalIdentifierId { get; private set; }
  public Guid Id { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int RealmId { get; private set; }

  public UserEntity? User { get; private set; }
  public int UserId { get; private set; }

  public string Key { get; private set; } = string.Empty;
  public string Value { get; private set; } = string.Empty;
  public string ValueNormalized
  {
    get => Value.ToUpper();
    private set { }
  }

  public Guid CreatedById { get; private set; }
  public string CreatedBy { get; private set; } = string.Empty;
  public DateTime CreatedOn { get; private set; }

  public Guid? UpdatedById { get; private set; }
  public string? UpdatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }

  public void SetActor(Guid id, ActorEntity actor)
  {
    if (CreatedById == id)
    {
      CreatedBy = actor.Serialize();
    }

    if (UpdatedById == id)
    {
      UpdatedBy = actor.Serialize();
    }
  }

  public void Update(ExternalIdentifierSet e, ActorEntity actor)
  {
    Value = e.Value ?? throw new ArgumentException($"The {nameof(e.Value)} is required.", nameof(e));

    UpdatedById = e.ActorId.ToGuid();
    UpdatedBy = actor.Serialize();
    UpdatedOn = e.OccurredOn;
  }
}
