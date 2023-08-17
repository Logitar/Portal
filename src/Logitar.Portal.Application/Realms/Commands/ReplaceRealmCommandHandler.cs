using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal class ReplaceRealmCommandHandler : IRequestHandler<ReplaceRealmCommand, Realm?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public ReplaceRealmCommandHandler(IApplicationContext applicationContext,
    IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _applicationContext = applicationContext;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<Realm?> Handle(ReplaceRealmCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = command.Id.GetAggregateId(nameof(command.Id));
    RealmAggregate? realm = await _realmRepository.LoadAsync(id, cancellationToken);
    if (realm == null)
    {
      return null;
    }

    RealmAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _realmRepository.LoadAsync(id, command.Version.Value, cancellationToken);
    }

    ReplaceRealmPayload payload = command.Payload;
    CultureInfo? defaultLocale = payload.DefaultLocale?.GetCultureInfo(nameof(payload.DefaultLocale));
    Uri? url = payload.Url?.GetUrl(nameof(payload.Url));
    ReadOnlyUniqueNameSettings uniqueNameSettings = payload.UniqueNameSettings.ToReadOnlyUniqueNameSettings();
    ReadOnlyPasswordSettings passwordSettings = payload.PasswordSettings.ToReadOnlyPasswordSettings();

    if (reference == null || payload.UniqueSlug != reference.UniqueSlug)
    {
      RealmAggregate? other = await _realmRepository.LoadAsync(payload.UniqueSlug, cancellationToken);
      if (other?.Equals(realm) == false)
      {
        throw new UniqueSlugAlreadyUsedException(payload.UniqueSlug, nameof(payload.UniqueSlug));
      }

      realm.UniqueSlug = payload.UniqueSlug;
    }
    if (reference == null || payload.DisplayName?.CleanTrim() != reference.DisplayName)
    {
      realm.DisplayName = payload.DisplayName;
    }
    if (reference == null || payload.Description?.CleanTrim() != reference.Description)
    {
      realm.Description = payload.Description;
    }

    if (reference == null || defaultLocale != reference.DefaultLocale)
    {
      realm.DefaultLocale = defaultLocale;
    }
    if (reference == null || payload.Secret != reference.Secret)
    {
      realm.Secret = payload.Secret;
    }
    if (reference == null || url != reference.Url)
    {
      realm.Url = url;
    }

    if (reference == null || payload.RequireUniqueEmail != reference.RequireUniqueEmail)
    {
      realm.RequireUniqueEmail = payload.RequireUniqueEmail;
    }
    if (reference == null || payload.RequireConfirmedAccount != reference.RequireConfirmedAccount)
    {
      realm.RequireConfirmedAccount = payload.RequireConfirmedAccount;
    }

    if (reference == null || uniqueNameSettings != reference.UniqueNameSettings)
    {
      realm.UniqueNameSettings = uniqueNameSettings;
    }
    if (reference == null || passwordSettings != reference.PasswordSettings)
    {
      realm.PasswordSettings = passwordSettings;
    }

    HashSet<string> claimMappingKeys = payload.ClaimMappings.Select(x => x.Key).ToHashSet();
    foreach (string claimMappingKey in realm.ClaimMappings.Keys)
    {
      if (!claimMappingKeys.Contains(claimMappingKey)
        && (reference == null || reference.ClaimMappings.ContainsKey(claimMappingKey)))
      {
        realm.RemoveClaimMapping(claimMappingKey);
      }
    }
    foreach (ClaimMapping claimMapping in payload.ClaimMappings)
    {
      realm.SetClaimMapping(claimMapping.Key, new ReadOnlyClaimMapping(claimMapping.Name, claimMapping.Type));
    }

    HashSet<string> customAttributeKeys = payload.CustomAttributes.Select(x => x.Key).ToHashSet();
    foreach (string customAttributeKey in realm.CustomAttributes.Keys)
    {
      if (!customAttributeKeys.Contains(customAttributeKey)
        && (reference == null || reference.CustomAttributes.ContainsKey(customAttributeKey)))
      {
        realm.RemoveCustomAttribute(customAttributeKey);
      }
    }
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      realm.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }

    if (realm.HasChanges)
    {
      realm.Update(_applicationContext.ActorId);

      await _realmRepository.SaveAsync(realm, cancellationToken);
    }

    return await _realmQuerier.ReadAsync(realm, cancellationToken);
  }
}
