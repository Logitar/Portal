using Logitar.Portal.Contracts.Actors;

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
    public ActorEntity(ApiKeyEntity apiKey)
    {
      Type = ActorType.ApiKey;
      AggregateId = apiKey.AggregateId;

      DisplayName = apiKey.DisplayName;
    }
    private ActorEntity()
    {
    }

    public int ActorId { get; private set; }

    public ActorType Type { get; private set; }
    public string AggregateId { get; private set; } = string.Empty;
    public bool IsDeleted { get; private set; }

    public string DisplayName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Picture { get; private set; }

    public void Delete() => IsDeleted = true;
    public void Update(ApiKeyEntity apiKey)
    {
      if (Type != ActorType.ApiKey)
      {
        throw new InvalidOperationException($"This actor has type '{Type}' and cannot be updated by an '{ActorType.ApiKey}' actor.");
      }

      DisplayName = apiKey.DisplayName;
    }
    public void Update(UserEntity user)
    {
      if (Type != ActorType.User)
      {
        throw new InvalidOperationException($"This actor has type '{Type}' and cannot be updated by an '{ActorType.User}' actor.");
      }

      DisplayName = user.FullName ?? user.Username;
      Email = user.Email;
      Picture = user.Picture;
    }
  }
}
