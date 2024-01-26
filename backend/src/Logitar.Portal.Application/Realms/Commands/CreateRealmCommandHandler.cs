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

internal class CreateRealmCommandHandler : IRequestHandler<CreateRealmCommand, Realm>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public CreateRealmCommandHandler(IApplicationContext applicationContext, IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _applicationContext = applicationContext;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<Realm> Handle(CreateRealmCommand command, CancellationToken cancellationToken)
  {
    CreateRealmPayload payload = command.Payload;
    new CreateRealmValidator().ValidateAndThrow(payload);

    RealmId? realmId = RealmId.TryCreate(payload.Id);
    if (realmId != null && await _realmRepository.LoadAsync(realmId, includeDeleted: true, cancellationToken) != null)
    {
      throw new IdentifierAlreadyUsedException<RealmAggregate>(payload.Id!, nameof(payload.Id));
    }

    UniqueSlugUnit uniqueSlug = new(payload.UniqueSlug);
    ActorId actorId = _applicationContext.ActorId;
    RealmAggregate realm = new(uniqueSlug, actorId, realmId)
    {
      DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName),
      Description = DescriptionUnit.TryCreate(payload.Description),
      DefaultLocale = LocaleUnit.TryCreate(payload.DefaultLocale),
      Secret = JwtSecretUnit.CreateOrGenerate(payload.Secret),
      Url = UrlUnit.TryCreate(payload.Url),
      UniqueNameSettings = new ReadOnlyUniqueNameSettings(payload.UniqueNameSettings),
      PasswordSettings = new ReadOnlyPasswordSettings(payload.PasswordSettings),
      RequireUniqueEmail = payload.RequireUniqueEmail
    };
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      realm.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    realm.Update(actorId);

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return await _realmQuerier.ReadAsync(realm, cancellationToken);
  }
}
