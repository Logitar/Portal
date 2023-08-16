using Logitar.Identity.Domain.Users;
using Logitar.Identity.Domain.Users.Events;
using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record ActorEntity
{
  public ActorEntity(UserCreatedEvent created)
  {
    Id = created.AggregateId.Value;
    Type = ActorType.User;
    DisplayName = created.UniqueName;
  }

  private ActorEntity()
  {
  }

  public long ActorId { get; private set; }

  public string Id { get; private set; } = string.Empty;
  public ActorType Type { get; private set; }
  public bool IsDeleted { get; private set; }

  public string DisplayName { get; private set; } = string.Empty;
  public string? EmailAddress { get; private set; }
  public string? PictureUrl { get; private set; }

  public void Delete(UserDeletedEvent _)
  {
    if (Type != ActorType.User)
    {
      throw new InvalidOperationException($"An event of type '{nameof(UserDeletedEvent)}' cannot be used to delete an actor of type '{Type}'.");
    }

    IsDeleted = true;
  }
  public void Update(UserAggregate user)
  {
    if (Type != ActorType.User)
    {
      throw new InvalidOperationException($"An aggregate of type '{nameof(UserAggregate)}' cannot be used to update an actor of type '{Type}'.");
    }

    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture?.ToString();
  }
}
