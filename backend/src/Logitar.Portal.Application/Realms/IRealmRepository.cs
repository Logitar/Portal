using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Realms
{
  public interface IRealmRepository : IRepository
  {
    Task<Realm?> LoadByAliasOrIdAsync(string aliasOrId, CancellationToken cancellationToken = default);
  }
}
