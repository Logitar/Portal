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

internal class UpdateRealmCommandHandler : IRequestHandler<UpdateRealmCommand, Realm?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public UpdateRealmCommandHandler(IApplicationContext applicationContext, IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _applicationContext = applicationContext;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<Realm?> Handle(UpdateRealmCommand command, CancellationToken cancellationToken)
  {
    UpdateRealmPayload payload = command.Payload;
    new UpdateRealmValidator().ValidateAndThrow(payload);

    RealmId realmId = new(command.Id);
    RealmAggregate? realm = await _realmRepository.LoadAsync(realmId, cancellationToken);
    if (realm == null)
    {
      return null;
    }

    ActorId actorId = _applicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.UniqueSlug))
    {
      UniqueSlugUnit uniqueName = new(payload.UniqueSlug);
      realm.SetUniqueSlug(uniqueName, actorId);
    }
    if (payload.DisplayName != null)
    {
      realm.DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      realm.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    if (payload.DefaultLocale != null)
    {
      realm.DefaultLocale = LocaleUnit.TryCreate(payload.DefaultLocale.Value);
    }
    if (payload.Secret != null)
    {
      realm.Secret = JwtSecretUnit.CreateOrGenerate(payload.Secret.Value);
    }
    if (payload.Url != null)
    {
      realm.Url = UrlUnit.TryCreate(payload.Url.Value);
    }

    if (payload.UniqueNameSettings != null)
    {
      realm.UniqueNameSettings = new ReadOnlyUniqueNameSettings(payload.UniqueNameSettings);
    }
    if (payload.PasswordSettings != null)
    {
      realm.PasswordSettings = new ReadOnlyPasswordSettings(payload.PasswordSettings);
    }
    if (payload.RequireUniqueEmail.HasValue)
    {
      realm.RequireUniqueEmail = payload.RequireUniqueEmail.Value;
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

    realm.Update(actorId);

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return await _realmQuerier.ReadAsync(realm, cancellationToken);
  }
}
