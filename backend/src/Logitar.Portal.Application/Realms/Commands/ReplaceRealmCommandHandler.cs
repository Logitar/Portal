using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Realms.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal class ReplaceRealmCommandHandler : IRequestHandler<ReplaceRealmCommand, Realm?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmManager _realmManager;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public ReplaceRealmCommandHandler(IApplicationContext applicationContext,
    IRealmManager realmManager, IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _applicationContext = applicationContext;
    _realmManager = realmManager;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<Realm?> Handle(ReplaceRealmCommand command, CancellationToken cancellationToken)
  {
    ReplaceRealmPayload payload = command.Payload;
    new ReplaceRealmValidator().ValidateAndThrow(payload);

    RealmAggregate? realm = await _realmRepository.LoadAsync(command.Id, cancellationToken);
    if (realm == null)
    {
      return null;
    }
    RealmAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _realmRepository.LoadAsync(realm.Id, command.Version.Value, cancellationToken);
    }

    ActorId actorId = _applicationContext.ActorId;

    UniqueSlugUnit uniqueSlug = new(payload.UniqueSlug);
    if (reference == null || uniqueSlug != reference.UniqueSlug)
    {
      realm.SetUniqueSlug(uniqueSlug, actorId);
    }
    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    if (reference == null || displayName != reference.DisplayName)
    {
      realm.DisplayName = displayName;
    }
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
    {
      realm.Description = description;
    }

    LocaleUnit? defaultLocale = LocaleUnit.TryCreate(payload.DefaultLocale);
    if (reference == null || defaultLocale != reference.DefaultLocale)
    {
      realm.DefaultLocale = defaultLocale;
    }
    JwtSecretUnit secret = JwtSecretUnit.CreateOrGenerate(payload.Secret);
    if (reference == null || secret != reference.Secret)
    {
      realm.Secret = secret;
    }
    UrlUnit? url = UrlUnit.TryCreate(payload.Url);
    if (reference == null || url != reference.Url)
    {
      realm.Url = url;
    }

    ReadOnlyUniqueNameSettings uniqueNameSettings = new(payload.UniqueNameSettings);
    if (reference == null || uniqueNameSettings != reference.UniqueNameSettings)
    {
      realm.UniqueNameSettings = uniqueNameSettings;
    }
    ReadOnlyPasswordSettings passwordSettings = new(payload.PasswordSettings);
    if (reference == null || passwordSettings != reference.PasswordSettings)
    {
      realm.PasswordSettings = passwordSettings;
    }
    if (reference == null || payload.RequireUniqueEmail != reference.RequireUniqueEmail)
    {
      realm.RequireUniqueEmail = payload.RequireUniqueEmail;
    }

    ReplaceCustomAttributes(payload, realm, reference);

    realm.Update(actorId);
    await _realmManager.SaveAsync(realm, actorId, cancellationToken);

    return await _realmQuerier.ReadAsync(realm, cancellationToken);
  }

  private static void ReplaceCustomAttributes(ReplaceRealmPayload payload, RealmAggregate role, RealmAggregate? reference)
  {
    HashSet<string> payloadKeys = new(capacity: payload.CustomAttributes.Count);

    IEnumerable<string> referenceKeys;
    if (reference == null)
    {
      referenceKeys = role.CustomAttributes.Keys;

      foreach (CustomAttribute customAttribute in payload.CustomAttributes)
      {
        payloadKeys.Add(customAttribute.Key.Trim());
        role.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
      }
    }
    else
    {
      referenceKeys = reference.CustomAttributes.Keys;

      foreach (CustomAttribute customAttribute in payload.CustomAttributes)
      {
        string key = customAttribute.Key.Trim();
        payloadKeys.Add(key);

        string value = customAttribute.Value.Trim();
        if (!reference.CustomAttributes.TryGetValue(key, out string? existingValue) || value != existingValue)
        {
          role.SetCustomAttribute(key, value);
        }
      }
    }

    foreach (string key in referenceKeys)
    {
      if (!payloadKeys.Contains(key))
      {
        role.RemoveCustomAttribute(key);
      }
    }
  }
}
