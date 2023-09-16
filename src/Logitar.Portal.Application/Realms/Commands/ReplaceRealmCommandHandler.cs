using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal class ReplaceRealmCommandHandler : IRequestHandler<ReplaceRealmCommand, Realm?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;
  private readonly ISenderRepository _senderRepository;
  private readonly ITemplateRepository _templateRepository;

  public ReplaceRealmCommandHandler(IApplicationContext applicationContext, IRealmQuerier realmQuerier,
    IRealmRepository realmRepository, ISenderRepository senderRepository, ITemplateRepository templateRepository)
  {
    _applicationContext = applicationContext;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
    _senderRepository = senderRepository;
    _templateRepository = templateRepository;
  }

  public async Task<Realm?> Handle(ReplaceRealmCommand command, CancellationToken cancellationToken)
  {
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

    ReplaceRealmPayload payload = command.Payload;
    ReadOnlyLocale? defaultLocale = payload.DefaultLocale?.GetLocale(nameof(payload.DefaultLocale));
    Uri? url = payload.Url?.GetUrl(nameof(payload.Url));
    ReadOnlyUniqueNameSettings uniqueNameSettings = payload.UniqueNameSettings.ToReadOnlyUniqueNameSettings();
    ReadOnlyPasswordSettings passwordSettings = payload.PasswordSettings.ToReadOnlyPasswordSettings();

    SenderAggregate? sender = null;
    if (payload.PasswordRecoverySenderId.HasValue)
    {
      sender = await _senderRepository.LoadAsync(payload.PasswordRecoverySenderId.Value, cancellationToken)
        ?? throw new AggregateNotFoundException<SenderAggregate>(payload.PasswordRecoverySenderId.Value, nameof(payload.PasswordRecoverySenderId));
    }
    TemplateAggregate? template = null;
    if (payload.PasswordRecoveryTemplateId.HasValue)
    {
      template = await _templateRepository.LoadAsync(payload.PasswordRecoveryTemplateId.Value, cancellationToken)
        ?? throw new AggregateNotFoundException<TemplateAggregate>(payload.PasswordRecoveryTemplateId.Value, nameof(payload.PasswordRecoveryTemplateId));
    }

    if (reference == null || payload.UniqueSlug.Trim() != reference.UniqueSlug)
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
    if (reference == null || payload.Secret.Trim() != reference.Secret)
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

    HashSet<string> claimMappingKeys = payload.ClaimMappings.Select(x => x.Key.Trim()).ToHashSet();
    foreach (string key in realm.ClaimMappings.Keys)
    {
      if (!claimMappingKeys.Contains(key) && (reference == null || reference.ClaimMappings.ContainsKey(key)))
      {
        realm.RemoveClaimMapping(key);
      }
    }
    foreach (ClaimMapping claimMapping in payload.ClaimMappings)
    {
      realm.SetClaimMapping(claimMapping.Key, new ReadOnlyClaimMapping(claimMapping.Name, claimMapping.Type));
    }

    HashSet<string> customAttributeKeys = payload.CustomAttributes.Select(x => x.Key.Trim()).ToHashSet();
    foreach (string key in realm.CustomAttributes.Keys)
    {
      if (!customAttributeKeys.Contains(key) && (reference == null || reference.CustomAttributes.ContainsKey(key)))
      {
        realm.RemoveCustomAttribute(key);
      }
    }
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      realm.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }

    if (reference == null || sender?.Id != reference.PasswordRecoverySenderId)
    {
      if (sender == null)
      {
        realm.RemovePasswordRecoverySender();
      }
      else
      {
        realm.SetPasswordRecoverySender(sender);
      }
    }
    if (reference == null || template?.Id != reference.PasswordRecoveryTemplateId)
    {
      if (template == null)
      {
        realm.RemovePasswordRecoveryTemplate();
      }
      else
      {
        realm.SetPasswordRecoveryTemplate(template);
      }
    }

    realm.Update(_applicationContext.ActorId);

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return await _realmQuerier.ReadAsync(realm, cancellationToken);
  }
}
