using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal class UpdateRealmCommandHandler : IRequestHandler<UpdateRealmCommand, Realm?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;
  private readonly ISenderRepository _senderRepository;
  private readonly ITemplateRepository _templateRepository;

  public UpdateRealmCommandHandler(IApplicationContext applicationContext, IRealmQuerier realmQuerier,
    IRealmRepository realmRepository, ISenderRepository senderRepository, ITemplateRepository templateRepository)
  {
    _applicationContext = applicationContext;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
    _senderRepository = senderRepository;
    _templateRepository = templateRepository;
  }

  public async Task<Realm?> Handle(UpdateRealmCommand command, CancellationToken cancellationToken)
  {
    RealmAggregate? realm = await _realmRepository.LoadAsync(command.Id, cancellationToken);
    if (realm == null)
    {
      return null;
    }

    UpdateRealmPayload payload = command.Payload;

    if (payload.UniqueSlug != null)
    {
      RealmAggregate? other = await _realmRepository.LoadAsync(payload.UniqueSlug, cancellationToken);
      if (other?.Equals(realm) == false)
      {
        throw new UniqueSlugAlreadyUsedException(payload.UniqueSlug, nameof(payload.UniqueSlug));
      }

      realm.UniqueSlug = payload.UniqueSlug;
    }
    if (payload.DisplayName != null)
    {
      realm.DisplayName = payload.DisplayName.Value;
    }
    if (payload.Description != null)
    {
      realm.Description = payload.Description.Value;
    }

    if (payload.DefaultLocale != null)
    {
      realm.DefaultLocale = payload.DefaultLocale.Value?.GetLocale(nameof(payload.DefaultLocale));
    }
    if (payload.Secret != null)
    {
      if (string.IsNullOrWhiteSpace(payload.Secret))
      {
        realm.GenerateNewSecret();
      }
      else
      {
        realm.Secret = new JwtSecret(payload.Secret);
      }
    }
    if (payload.Url != null)
    {
      realm.Url = payload.Url.Value?.GetUrl(nameof(payload.Url));
    }

    if (payload.RequireUniqueEmail.HasValue)
    {
      realm.RequireUniqueEmail = payload.RequireUniqueEmail.Value;
    }
    if (payload.RequireConfirmedAccount.HasValue)
    {
      realm.RequireConfirmedAccount = payload.RequireConfirmedAccount.Value;
    }

    if (payload.UniqueNameSettings != null)
    {
      realm.UniqueNameSettings = payload.UniqueNameSettings.ToReadOnlyUniqueNameSettings();
    }
    if (payload.PasswordSettings != null)
    {
      realm.PasswordSettings = payload.PasswordSettings.ToReadOnlyPasswordSettings();
    }

    foreach (ClaimMappingModification claimMapping in payload.ClaimMappings)
    {
      if (claimMapping.Name == null)
      {
        realm.RemoveClaimMapping(claimMapping.Key);
      }
      else
      {
        realm.SetClaimMapping(claimMapping.Key, new ReadOnlyClaimMapping(claimMapping.Name, claimMapping.Type));
      }
    }

    foreach (CustomAttributeModification customAttribute in payload.CustomAttributes)
    {
      if (customAttribute.Value == null)
      {
        realm.RemoveCustomAttribute(customAttribute.Key);
      }
      else
      {
        realm.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
      }
    }

    if (payload.PasswordRecoverySenderId != null)
    {
      if (payload.PasswordRecoverySenderId.Value.HasValue)
      {
        Guid senderId = payload.PasswordRecoverySenderId.Value.Value;
        SenderAggregate sender = await _senderRepository.LoadAsync(senderId, cancellationToken)
          ?? throw new AggregateNotFoundException<SenderAggregate>(senderId, nameof(payload.PasswordRecoverySenderId));
        realm.SetPasswordRecoverySender(sender);
      }
      else
      {
        realm.RemovePasswordRecoverySender();
      }
    }
    if (payload.PasswordRecoveryTemplateId != null)
    {
      if (payload.PasswordRecoveryTemplateId.Value.HasValue)
      {
        Guid templateId = payload.PasswordRecoveryTemplateId.Value.Value;
        TemplateAggregate template = await _templateRepository.LoadAsync(templateId, cancellationToken)
          ?? throw new AggregateNotFoundException<TemplateAggregate>(templateId, nameof(payload.PasswordRecoveryTemplateId));
        realm.SetPasswordRecoveryTemplate(template);
      }
      else
      {
        realm.RemovePasswordRecoveryTemplate();
      }
    }

    realm.Update(_applicationContext.ActorId);

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return await _realmQuerier.ReadAsync(realm, cancellationToken);
  }
}
