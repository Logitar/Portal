using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.ApiKeys.Events;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.Domain.Users.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record ActorEntity
{
  public ActorEntity(ApiKeyAggregate apiKey)
  {
    Id = apiKey.Id.ToGuid();
    Type = ActorType.ApiKey;
    IsDeleted = apiKey.IsDeleted;

    DisplayName = apiKey.Title;
  }
  public ActorEntity(ApiKeyCreatedEvent created)
  {
    Id = created.AggregateId.ToGuid();
    Type = ActorType.ApiKey;

    DisplayName = created.Title;
  }

  public ActorEntity(UserAggregate user)
  {
    Id = user.Id.ToGuid();
    Type = ActorType.User;
    IsDeleted = user.IsDeleted;

    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture?.ToString();
  }
  public ActorEntity(UserCreatedEvent created)
  {
    Id = created.AggregateId.ToGuid();
    Type = ActorType.User;

    DisplayName = created.UniqueName;
  }

  private ActorEntity()
  {
  }

  public long ActorId { get; private set; }

  public Guid Id { get; private set; }
  public ActorType Type { get; private set; }
  public bool IsDeleted { get; private set; }

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

  public void Delete(ApiKeyDeletedEvent _)
  {
    if (Type != ActorType.ApiKey)
    {
      throw new InvalidOperationException($"An {nameof(ApiKeyDeletedEvent)} cannot be used to delete an actor of type '{Type}'.");
    }

    IsDeleted = true;
  }
  public void Delete(UserDeletedEvent _)
  {
    if (Type != ActorType.User)
    {
      throw new InvalidOperationException($"An {nameof(UserDeletedEvent)} cannot be used to delete an actor of type '{Type}'.");
    }

    IsDeleted = true;
  }

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
  public void Update(UserAggregate user)
  {
    if (Type != ActorType.User)
    {
      throw new InvalidOperationException($"An user cannot be used to update an actor of type '{Type}'.");
    }

    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture?.ToString();
  }
}
