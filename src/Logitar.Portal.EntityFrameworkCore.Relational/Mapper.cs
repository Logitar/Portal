using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal class Mapper
{
  private readonly IReadOnlyDictionary<ActorId, Actor> _actors;

  public Mapper(IReadOnlyDictionary<ActorId, Actor> actors)
  {
    _actors = actors;
  }

  public Realm Map(RealmEntity source)
  {
    Realm destination = new()
    {
      UniqueSlug = source.UniqueSlug,
      DisplayName = source.DisplayName,
      Description = source.Description,
      DefaultLocale = source.DefaultLocale,
      Secret = source.Secret,
      Url = source.Url,
      RequireUniqueEmail = source.RequireUniqueEmail,
      RequireConfirmedAccount = source.RequireConfirmedAccount,
      UniqueNameSettings = ToUniqueNameSettings(source.UniqueNameSettings),
      PasswordSettings = ToPasswordSettings(source.PasswordSettings),
      ClaimMappings = source.ClaimMappings.Select(claimMapping => new ClaimMapping(claimMapping.Key, claimMapping.Value.Name, claimMapping.Value.Type)),
      CustomAttributes = ToCustomAttributes(source.CustomAttributes)
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Id = source.AggregateId;
    destination.CreatedBy = GetActor(source.CreatedBy);
    destination.CreatedOn = ToUniversalTime(source.CreatedOn);
    destination.UpdatedBy = GetActor(source.UpdatedBy);
    destination.UpdatedOn = ToUniversalTime(source.UpdatedOn);
    destination.Version = source.Version;
  }
  private Actor GetActor(string id) => _actors.TryGetValue(new ActorId(id), out Actor? actor) ? actor : new();

  private static IEnumerable<CustomAttribute> ToCustomAttributes(Dictionary<string, string> customAttributes)
    => customAttributes.Select(customAttribute => new CustomAttribute(customAttribute.Key, customAttribute.Value));

  private static PasswordSettings ToPasswordSettings(ReadOnlyPasswordSettings source) => new()
  {
    RequiredLength = source.RequiredLength,
    RequiredUniqueChars = source.RequiredUniqueChars,
    RequireNonAlphanumeric = source.RequireNonAlphanumeric,
    RequireLowercase = source.RequireLowercase,
    RequireUppercase = source.RequireUppercase,
    RequireDigit = source.RequireDigit
  };

  private static UniqueNameSettings ToUniqueNameSettings(ReadOnlyUniqueNameSettings source) => new()
  {
    AllowedCharacters = source.AllowedCharacters
  };

  private static DateTime ToUniversalTime(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
}
