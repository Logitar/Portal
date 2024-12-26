using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
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

  public CreateRealmCommandHandler(IRealmManager realmManager, IRealmQuerier realmQuerier)
  {
    _realmManager = realmManager;
    _realmQuerier = realmQuerier;
  }

  public async Task<RealmModel> Handle(CreateRealmCommand command, CancellationToken cancellationToken)
  {
    CreateRealmPayload payload = command.Payload;
    new CreateRealmValidator().ValidateAndThrow(payload);

    ActorId actorId = command.ActorId;

    Slug uniqueSlug = new(payload.UniqueSlug);
    Realm realm = new(uniqueSlug, actorId)
    {
      DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName),
      Description = DescriptionUnit.TryCreate(payload.Description),
      DefaultLocale = LocaleUnit.TryCreate(payload.DefaultLocale),
      Url = UrlUnit.TryCreate(payload.Url),
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
      realm.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }

    realm.Update(actorId);
    await _realmManager.SaveAsync(realm, actorId, cancellationToken);

    return await _realmQuerier.ReadAsync(realm, cancellationToken);
  }
}
