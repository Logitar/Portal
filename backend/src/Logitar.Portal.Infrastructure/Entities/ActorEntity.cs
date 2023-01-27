using Logitar.Portal.Core.Actors;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class ActorEntity
  {
    public ActorEntity(UserEntity user)
    {
      Type = ActorType.User;
      AggregateId = user.AggregateId;

      DisplayName = user.FullName ?? user.Username;
      Email = user.Email;
      Picture = user.Picture;
    }
    private ActorEntity()
    {
    }

    public static ActorEntity System => new()
    {
      AggregateId = nameof(System),
      DisplayName = nameof(System)
    };

    public int ActorId { get; private set; }

    public ActorType Type { get; private set; }
    public string AggregateId { get; private set; } = null!;
    public bool IsDeleted { get; private set; }

    public string DisplayName { get; private set; } = null!;
    public string? Email { get; private set; }
    public string? Picture { get; private set; }

    public void Delete() => IsDeleted = true;

    public void Update(UserEntity user)
    {
      DisplayName = user.FullName ?? user.Username;
      Email = user.Email;
      Picture = user.Picture;
    }
  }
}
