namespace Logitar.Portal.v2.Contracts.Realms;

public interface IRealmService
{
  Task<Realm> CreateAsync(CreateRealmInput input, CancellationToken cancellationToken = default);
}
