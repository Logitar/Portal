using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal class Mapper
{
  private static readonly Actor _system = new();

  private readonly Dictionary<ActorId, Actor> _actors;

  public Mapper()
  {
    _actors = [];
  }

  public Mapper(IEnumerable<Actor> actors) : this()
  {
    foreach (Actor actor in actors)
    {
      ActorId id = new(actor.Id);
      _actors[id] = actor;
    }
  }

  public static Actor ToActor(ActorEntity actor) => new()
  {
    Id = actor.Id,
    Type = Enum.Parse<ActorType>(actor.Type),
    IsDeleted = actor.IsDeleted,
    DisplayName = actor.DisplayName,
    EmailAddress = actor.EmailAddress,
    PictureUrl = actor.PictureUrl
  };

  public Configuration ToConfiguration(ConfigurationAggregate configuration) => new(configuration.Secret.Value)
  {
    Id = configuration.Id.Value,
    Version = configuration.Version,
    CreatedBy = FindActor(configuration.CreatedBy),
    CreatedOn = configuration.CreatedOn.ToUniversalTime(),
    UpdatedBy = FindActor(configuration.UpdatedBy),
    UpdatedOn = configuration.UpdatedOn.ToUniversalTime(),
    DefaultLocale = configuration.DefaultLocale?.Code,
    UniqueNameSettings = new UniqueNameSettings
    {
      AllowedCharacters = configuration.UniqueNameSettings.AllowedCharacters
    },
    PasswordSettings = new PasswordSettings
    {
      RequiredLength = configuration.PasswordSettings.RequiredLength,
      RequiredUniqueChars = configuration.PasswordSettings.RequiredUniqueChars,
      RequireNonAlphanumeric = configuration.PasswordSettings.RequireNonAlphanumeric,
      RequireLowercase = configuration.PasswordSettings.RequireLowercase,
      RequireUppercase = configuration.PasswordSettings.RequireUppercase,
      RequireDigit = configuration.PasswordSettings.RequireDigit,
      HashingStrategy = configuration.PasswordSettings.HashingStrategy
    },
    RequireUniqueEmail = configuration.RequireUniqueEmail
  };

  private Actor FindActor(string id) => FindActor(new ActorId(id));
  private Actor FindActor(ActorId id) => _actors.TryGetValue(id, out Actor? actor) ? actor : _system;
  private Actor? TryFindActor(string? id) => id == null ? null : FindActor(id);
  private Actor? TryFindActor(ActorId? id) => id.HasValue ? FindActor(id.Value) : null;

  private static DateTime? AsUniversalTime(DateTime? value) => value.HasValue ? AsUniversalTime(value.Value) : null;
  private static DateTime AsUniversalTime(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
}
