using Logitar.Portal.Contracts.Actors;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;

internal record ActorEntity
{
  private static readonly JsonSerializerOptions _options = new();
  static ActorEntity() => _options.Converters.Add(new JsonStringEnumConverter());

  public static ActorEntity System { get; } = new();

  public ActorType Type { get; init; }
  public bool IsDeleted { get; init; }

  public string DisplayName { get; init; } = ActorType.System.ToString();
  public string? EmailAddress { get; init; }
  public string? Picture { get; init; }

  public static ActorEntity Deserialize(string json) => JsonSerializer.Deserialize<ActorEntity>(json, _options)
    ?? throw new InvalidOperationException($"The actor deserialization failed: '{json}'.");

  public static ActorEntity From(Actor actor) => new()
  {
    Type = actor.Type,
    IsDeleted = actor.IsDeleted,
    DisplayName = actor.DisplayName,
    EmailAddress = actor.EmailAddress,
    Picture = actor.Picture
  };
  public static ActorEntity From(UserEntity user, bool isDeleted = false) => new()
  {
    Type = ActorType.User,
    IsDeleted = isDeleted,
    DisplayName = user.FullName ?? user.Username,
    EmailAddress = user.EmailAddress,
    Picture = user.Picture
  };

  public string Serialize() => JsonSerializer.Serialize(this, _options);

  public Actor ToActor(Guid id) => new()
  {
    Id = id,
    Type = Type,
    IsDeleted = IsDeleted,
    DisplayName = DisplayName,
    EmailAddress = EmailAddress,
    Picture = Picture
  };
}
