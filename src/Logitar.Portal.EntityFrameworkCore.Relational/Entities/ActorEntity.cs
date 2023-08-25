using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Domain.Users.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record ActorEntity
{
  public ActorEntity(UserCreatedEvent created)
  {
    Id = created.AggregateId.Value;
    Type = ActorType.User;

    DisplayName = created.UniqueName;
  }
  public ActorEntity(UserEntity user)
  {
    Id = user.AggregateId;
    Type = ActorType.User;

    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.EmailAddress;
    PictureUrl = user.Picture;
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

  public Actor ToActor() => new()
  {
    Id = Id,
    Type = Type,
    IsDeleted = IsDeleted,
    DisplayName = DisplayName,
    EmailAddress = EmailAddress,
    PictureUrl = PictureUrl
  };

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
