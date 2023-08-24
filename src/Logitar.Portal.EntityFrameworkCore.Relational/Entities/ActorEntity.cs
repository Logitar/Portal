using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record ActorEntity
{
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
}
