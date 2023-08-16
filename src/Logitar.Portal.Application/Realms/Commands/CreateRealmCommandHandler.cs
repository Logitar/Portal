using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
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

    CultureInfo? defaultLocale = payload.DefaultLocale?.GetCultureInfo(nameof(payload.DefaultLocale));
    Uri? url = payload.Url?.GetUrl(nameof(payload.Url));

    realm = new(payload.UniqueSlug, _applicationContext.ActorId)
    {
      DisplayName = payload.DisplayName,
      Description = payload.Description,
      DefaultLocale = defaultLocale,
      Url = url,
      RequireUniqueEmail = payload.RequireUniqueEmail,
      RequireConfirmedAccount = payload.RequireConfirmedAccount
    };

    if (payload.Secret != null)
    {
      realm.Secret = payload.Secret;
    }

    if (payload.UniqueNameSettings != null)
    {
      realm.UniqueNameSettings = payload.UniqueNameSettings.ToReadOnlyUniqueNameSettings();
    }
    if (payload.PasswordSettings != null)
    {
      realm.PasswordSettings = payload.PasswordSettings.ToReadOnlyPasswordSettings();
    }

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
