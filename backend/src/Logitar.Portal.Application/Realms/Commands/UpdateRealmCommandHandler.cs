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
  private readonly IRealmManager _realmManager;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public UpdateRealmCommandHandler(IRealmManager realmManager, IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _realmManager = realmManager;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<Realm?> Handle(UpdateRealmCommand command, CancellationToken cancellationToken)
  {
    UpdateRealmPayload payload = command.Payload;
    new UpdateRealmValidator().ValidateAndThrow(payload);

    RealmAggregate? realm = await _realmRepository.LoadAsync(command.Id, cancellationToken);
    if (realm == null)
    {
      return null;
    }

    ActorId actorId = command.ActorId;

    UniqueSlugUnit? uniqueSlug = UniqueSlugUnit.TryCreate(payload.UniqueSlug);
    if (uniqueSlug != null)
    {
      realm.SetUniqueSlug(uniqueSlug, actorId);
    }
    if (payload.DisplayName != null)
    {
      realm.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      realm.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.DefaultLocale != null)
    {
      realm.DefaultLocale = LocaleUnit.TryCreate(payload.DefaultLocale.Value);
    }
    if (payload.Secret != null)
    {
      realm.Secret = JwtSecretUnit.CreateOrGenerate(payload.Secret);
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
      if (string.IsNullOrWhiteSpace(customAttribute.Value))
      {
        realm.RemoveCustomAttribute(customAttribute.Key);
      }
      else
      {
        realm.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
      }
    }

    realm.Update(actorId);
    await _realmManager.SaveAsync(realm, actorId, cancellationToken);

    return await _realmQuerier.ReadAsync(realm, cancellationToken);
  }
}
