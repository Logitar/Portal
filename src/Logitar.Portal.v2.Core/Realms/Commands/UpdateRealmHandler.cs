using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Realms;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.v2.Core.Realms.Commands;

internal class UpdateRealmHandler : IRequestHandler<UpdateRealm, Realm>
{
  private readonly ICurrentActor _currentActor;
  private readonly IEventStore _eventStore;
  private readonly IRealmQuerier _realmQuerier;

  public UpdateRealmHandler(ICurrentActor currentActor,
    IEventStore eventStore,
    IRealmQuerier realmQuerier)
  {
    _currentActor = currentActor;
    _eventStore = eventStore;
    _realmQuerier = realmQuerier;
  }

  public async Task<Realm> Handle(UpdateRealm request, CancellationToken cancellationToken)
  {
    RealmAggregate realm = await _eventStore.LoadAsync<RealmAggregate>(new AggregateId(request.Id), cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(request.Id);

    UpdateRealmInput input = request.Input;

    CultureInfo? defaultLocale = input.DefaultLocale?.GetCultureInfo(nameof(input.DefaultLocale));
    Uri? url = input.Url?.GetUri(nameof(input.Url));
    ReadOnlyUsernameSettings? usernameSettings = input.UsernameSettings == null ? null : new(input.UsernameSettings);
    ReadOnlyPasswordSettings? passwordSettings = input.PasswordSettings == null ? null : new(input.PasswordSettings);

    realm.Update(_currentActor.Id, input.DisplayName, input.Description,
      defaultLocale, input.Secret, url,
      input.RequireConfirmedAccount, input.RequireUniqueEmail, usernameSettings, passwordSettings,
      input.ClaimMappings?.ToDictionary(), input.CustomAttributes?.ToDictionary());

    await _eventStore.SaveAsync(realm, cancellationToken);

    return await _realmQuerier.GetAsync(realm, cancellationToken);
  }
}
