using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Realms;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.v2.Core.Realms.Commands;

internal class CreateRealmHandler : IRequestHandler<CreateRealm, Realm>
{
  private readonly ICurrentActor _currentActor;
  private readonly IEventStore _eventStore;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public CreateRealmHandler(ICurrentActor currentActor,
    IEventStore eventStore,
    IRealmQuerier realmQuerier,
    IRealmRepository realmRepository)
  {
    _currentActor = currentActor;
    _eventStore = eventStore;
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
    ReadOnlyUsernameSettings? usernameSettings = input.UsernameSettings == null ? null : new(input.UsernameSettings);
    ReadOnlyPasswordSettings? passwordSettings = input.PasswordSettings == null ? null : new(input.PasswordSettings);

    RealmAggregate realm = new(_currentActor.Id, uniqueName, input.DisplayName, input.Description,
      defaultLocale, input.Secret, url,
      input.RequireConfirmedAccount, input.RequireUniqueEmail, usernameSettings, passwordSettings,
      input.ClaimMappings?.ToDictionary(), input.CustomAttributes?.ToDictionary());

    await _eventStore.SaveAsync(realm, cancellationToken);

    return await _realmQuerier.GetAsync(realm, cancellationToken);
  }
}
