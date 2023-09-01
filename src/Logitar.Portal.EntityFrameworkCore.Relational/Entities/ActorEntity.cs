using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Domain.ApiKeys.Events;
using Logitar.Portal.Domain.Users.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record ActorEntity
{
  public ActorEntity(ApiKeyCreatedEvent created)
  {
    Id = created.AggregateId.ToGuid();
    Type = ActorType.ApiKey;

    DisplayName = created.Title;
  }
  public ActorEntity(ApiKeyEntity user)
  {
    Id = new AggregateId(user.AggregateId).ToGuid();
    Type = ActorType.ApiKey;

    DisplayName = user.Title;
  }
  public ActorEntity(UserCreatedEvent created)
  {
    Id = created.AggregateId.ToGuid();
    Type = ActorType.User;

    DisplayName = created.UniqueName;
  }
  public ActorEntity(UserEntity user)
  {
    Id = new AggregateId(user.AggregateId).ToGuid();
    Type = ActorType.User;

    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.EmailAddress;
    PictureUrl = user.Picture;
  }

  private ActorEntity()
  {
  }

  public long ActorId { get; private set; }

  public Guid Id { get; private set; }
  public ActorType Type { get; private set; }
  public bool IsDeleted { get; private set; } // TODO(fpion): this is never set to true!

  public string DisplayName { get; private set; } = string.Empty;
  public string? EmailAddress { get; private set; }
  public string? PictureUrl { get; private set; }

  public Actor ToActor() => new()
  {
    Id = Id,
    Type = Type,
    IsDeleted = IsDeleted,
    DisplayName = DisplayName,
    EmailAddress = EmailAddress,
    PictureUrl = PictureUrl
  };

  public void Update(ApiKeyUpdatedEvent updated)
  {
    if (Type != ActorType.ApiKey)
    {
      throw new InvalidOperationException($"An {nameof(ApiKeyUpdatedEvent)} cannot be used to update an actor of type '{Type}'.");
    }

    if (updated.Title != null)
    {
      DisplayName = updated.Title;
    }
  }

  public void Update(UserEntity user)
  {
    if (Type != ActorType.User)
    {
      throw new InvalidOperationException($"An user cannot be used to update an actor of type '{Type}'.");
    }

    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.EmailAddress;
    PictureUrl = user.Picture;
  }
}
