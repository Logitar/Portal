namespace Logitar.Portal.v2.Core.Realms;

public interface IRealmRepository
{
  Task<RealmAggregate?> LoadByUniqueNameAsync(string uniqueName, CancellationToken cancellationToken = default);
}
