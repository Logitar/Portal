using Logitar.Portal.Contracts.Realms;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Realms.Commands;

internal class CreateRealmHandler : IRequestHandler<CreateRealm, Realm>
{
  private readonly ICurrentActor _currentActor;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public CreateRealmHandler(ICurrentActor currentActor,
    IRealmQuerier realmQuerier,
    IRealmRepository realmRepository)
  {
    _currentActor = currentActor;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<Realm> Handle(CreateRealm request, CancellationToken cancellationToken)
  {
    CreateRealmInput input = request.Input;

    string uniqueName = input.UniqueName.Trim();
    if (await _realmRepository.LoadByUniqueNameAsync(uniqueName, cancellationToken) != null)
    {
      throw new UniqueNameAlreadyUsedException(uniqueName, nameof(input.UniqueName));
    }

    CultureInfo? defaultLocale = input.DefaultLocale?.GetCultureInfo(nameof(input.DefaultLocale));
    Uri? url = input.Url?.GetUri(nameof(input.Url));
    ReadOnlyUsernameSettings? usernameSettings = ReadOnlyUsernameSettings.From(input.UsernameSettings);
    ReadOnlyPasswordSettings? passwordSettings = ReadOnlyPasswordSettings.From(input.PasswordSettings);

    RealmAggregate realm = new(_currentActor.Id, uniqueName, input.DisplayName, input.Description,
      defaultLocale, input.Secret, url,
      input.RequireConfirmedAccount, input.RequireUniqueEmail, usernameSettings, passwordSettings,
      input.ClaimMappings?.ToDictionary(), input.CustomAttributes?.ToDictionary());

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return await _realmQuerier.GetAsync(realm, cancellationToken);
  }
}
