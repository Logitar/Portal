using Logitar.Portal.Core.ApiKeys;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Actors
{
  public class Actor
  {
    public Actor(ApiKey apiKey)
    {
      ArgumentNullException.ThrowIfNull(apiKey);

      Id = apiKey.Id;

      Type = ActorType.ApiKey;
      Name = apiKey.Name;
      IsDeleted = apiKey.IsDeleted;
    }
    public Actor(User user)
    {
      ArgumentNullException.ThrowIfNull(user);

      Id = user.Id;

      Type = ActorType.User;
      Name = user.FullName ?? user.Username;
      IsDeleted = user.IsDeleted;

      Email = user.Email;
      Picture = user.Picture;
    }
    private Actor()
    {
    }

    public Guid Id { get; private set; }
    public int Sid { get; private set; }

    public ActorType Type { get; private set; }
    public string Name { get; private set; } = null!;
    public bool IsDeleted { get; private set; }

    public string? Email { get; private set; }
    public string? Picture { get; private set; }

    public void Update(ApiKey apiKey)
    {
      ArgumentNullException.ThrowIfNull(apiKey);

      Name = apiKey.Name;
      IsDeleted = apiKey.IsDeleted;
    }
    public void Update(User user)
    {
      ArgumentNullException.ThrowIfNull(user);

      Name = user.FullName ?? user.Username;
      IsDeleted = user.IsDeleted;

      Email = user.Email;
      Picture = user.Picture;
    }

    public override bool Equals(object? obj) => obj is Actor actor && actor.Id == Id;
    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
    public override string ToString() => $"{Name} | {Type} (Id={Id})";
  }
}
