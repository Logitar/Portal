using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Infrastructure.Entities;
using System.Text.Json;

namespace Logitar.Portal.Infrastructure
{
  internal class Actor
  {
    public Actor()
    {
    }
    public Actor(ActorModel actor)
    {
      Type = actor.Type;
      IsDeleted = actor.IsDeleted;

      DisplayName = actor.DisplayName;
      Email = actor.Email;
      Picture = actor.Picture;
    }
    public Actor(ApiKeyEntity apiKey, bool isDeleted = false)
    {
      Type = ActorType.ApiKey;
      IsDeleted = isDeleted;

      DisplayName = apiKey.Title;
    }
    public Actor(UserEntity user, bool isDeleted = false)
    {
      Type = ActorType.User;
      IsDeleted = isDeleted;

      DisplayName = user.FullName ?? user.Username;
      Email = user.Email;
      Picture = user.Picture;
    }

    public ActorType Type { get; set; }
    public bool IsDeleted { get; set; }

    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Picture { get; set; }

    public static ActorModel? GetActorModel(string? id, string? json)
    {
      if (id == null || json == null)
      {
        return null;
      }

      Actor? actor = JsonSerializer.Deserialize<Actor>(json);

      return actor == null ? null : new ActorModel
      {
        Id = id,
        Type = actor.Type,
        IsDeleted = actor.IsDeleted,
        DisplayName = actor.DisplayName,
        Email = actor.Email,
        Picture = actor.Picture
      };
    }

    public string Serialize() => JsonSerializer.Serialize(this);
  }
}
