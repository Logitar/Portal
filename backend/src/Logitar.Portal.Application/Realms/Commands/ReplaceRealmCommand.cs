using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Realms.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal record ReplaceRealmCommand(Guid Id, ReplaceRealmPayload Payload, long? Version) : Activity, IRequest<RealmModel?>;

internal class ReplaceRealmCommandHandler : IRequestHandler<ReplaceRealmCommand, RealmModel?>
{
  private readonly IRealmManager _realmManager;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public ReplaceRealmCommandHandler(IRealmManager realmManager, IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _realmManager = realmManager;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<RealmModel?> Handle(ReplaceRealmCommand command, CancellationToken cancellationToken)
  {
    ReplaceRealmPayload payload = command.Payload;
    new ReplaceRealmValidator().ValidateAndThrow(payload);

    RealmId realmId = new(command.Id);
    Realm? realm = await _realmRepository.LoadAsync(realmId, cancellationToken);
    if (realm == null)
    {
      return null;
    }
    Realm? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _realmRepository.LoadAsync(realm.Id, command.Version.Value, cancellationToken);
    }

    ActorId actorId = command.ActorId;

    Slug uniqueSlug = new(payload.UniqueSlug);
    if (reference == null || uniqueSlug != reference.UniqueSlug)
    {
      realm.SetUniqueSlug(uniqueSlug, actorId);
    }
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference == null || displayName != reference.DisplayName)
    {
      realm.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
    {
      realm.Description = description;
    }

    Locale? defaultLocale = Locale.TryCreate(payload.DefaultLocale);
    if (reference == null || defaultLocale != reference.DefaultLocale)
    {
      realm.DefaultLocale = defaultLocale;
    }
    JwtSecret secret = JwtSecret.CreateOrGenerate(payload.Secret);
    if (reference == null || secret != reference.Secret)
    {
      realm.Secret = secret;
    }
    Url? url = Url.TryCreate(payload.Url);
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

  private static void ReplaceCustomAttributes(ReplaceRealmPayload payload, Realm role, Realm? reference)
  {
    HashSet<Identifier> payloadKeys = new(capacity: payload.CustomAttributes.Count);

    IEnumerable<Identifier> referenceKeys;
    if (reference == null)
    {
      referenceKeys = role.CustomAttributes.Keys;

      foreach (CustomAttribute customAttribute in payload.CustomAttributes)
      {
        Identifier key = new(customAttribute.Key);
        payloadKeys.Add(key);
        role.SetCustomAttribute(key, customAttribute.Value);
      }
    }
    else
    {
      referenceKeys = reference.CustomAttributes.Keys;

      foreach (CustomAttribute customAttribute in payload.CustomAttributes)
      {
        Identifier key = new(customAttribute.Key);
        payloadKeys.Add(key);

        string value = customAttribute.Value.Trim();
        if (!reference.CustomAttributes.TryGetValue(key, out string? existingValue) || existingValue != value)
        {
          role.SetCustomAttribute(key, value);
        }
      }
    }

    foreach (Identifier key in referenceKeys)
    {
      if (!payloadKeys.Contains(key))
      {
        role.RemoveCustomAttribute(key);
      }
    }
  }
}
