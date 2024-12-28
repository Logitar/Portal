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

internal record CreateRealmCommand(CreateRealmPayload Payload) : Activity, IRequest<RealmModel>;

internal class CreateRealmCommandHandler : IRequestHandler<CreateRealmCommand, RealmModel>
{
  private readonly IRealmManager _realmManager;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public CreateRealmCommandHandler(IRealmManager realmManager, IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _realmManager = realmManager;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<RealmModel> Handle(CreateRealmCommand command, CancellationToken cancellationToken)
  {
    CreateRealmPayload payload = command.Payload;
    new CreateRealmValidator().ValidateAndThrow(payload);

    Realm? realm;
    RealmId? realmId = null;
    if (payload.Id.HasValue)
    {
      realmId = new(payload.Id.Value);
      realm = await _realmRepository.LoadAsync(realmId.Value, cancellationToken);
      if (realm != null)
      {
        throw new IdAlreadyUsedException(payload.Id.Value);
      }
    }

    ActorId actorId = command.ActorId;

    Slug uniqueSlug = new(payload.UniqueSlug);
    realm = new(uniqueSlug, actorId, realmId)
    {
      DisplayName = DisplayName.TryCreate(payload.DisplayName),
      Description = Description.TryCreate(payload.Description),
      DefaultLocale = Locale.TryCreate(payload.DefaultLocale),
      Url = Url.TryCreate(payload.Url),
      UniqueNameSettings = new ReadOnlyUniqueNameSettings(payload.UniqueNameSettings),
      PasswordSettings = new ReadOnlyPasswordSettings(payload.PasswordSettings),
      RequireUniqueEmail = payload.RequireUniqueEmail
    };
    if (!string.IsNullOrWhiteSpace(payload.Secret))
    {
      realm.Secret = new JwtSecret(payload.Secret);
    }

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      realm.SetCustomAttribute(key, customAttribute.Value);
    }

    realm.Update(actorId);
    await _realmManager.SaveAsync(realm, actorId, cancellationToken);

    return await _realmQuerier.ReadAsync(realm, cancellationToken);
  }
}
