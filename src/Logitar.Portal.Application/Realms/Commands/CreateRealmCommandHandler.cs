using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal class CreateRealmCommandHandler : IRequestHandler<CreateRealmCommand, Realm>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public CreateRealmCommandHandler(IApplicationContext applicationContext,
    IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _applicationContext = applicationContext;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<Realm> Handle(CreateRealmCommand command, CancellationToken cancellationToken)
  {
    CreateRealmPayload payload = command.Payload;

    RealmAggregate? realm = await _realmRepository.LoadAsync(payload.UniqueSlug, cancellationToken);
    if (realm != null)
    {
      throw new UniqueSlugAlreadyUsedException(payload.UniqueSlug, nameof(payload.UniqueSlug));
    }

    ReadOnlyUniqueNameSettings? uniqueNameSettings = payload.UniqueNameSettings?.ToUniqueNameSettings();
    ReadOnlyPasswordSettings? passwordSettings = payload.PasswordSettings?.ToPasswordSettings();
    realm = new(payload.UniqueSlug, payload.Secret, payload.RequireUniqueEmail,
      payload.RequireConfirmedAccount, uniqueNameSettings, passwordSettings, _applicationContext.ActorId)
    {
      DisplayName = payload.DisplayName,
      Description = payload.Description,
      DefaultLocale = payload.DefaultLocale?.GetLocale(nameof(payload.DefaultLocale)),
      Url = payload.Url?.GetUrl(nameof(payload.Url))
    };

    foreach (ClaimMapping claimMapping in payload.ClaimMappings)
    {
      realm.SetClaimMapping(claimMapping.Key, new ReadOnlyClaimMapping(claimMapping.Name, claimMapping.Type));
    }

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      realm.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }

    realm.Update(_applicationContext.ActorId);

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return await _realmQuerier.ReadAsync(realm, cancellationToken);
  }
}
