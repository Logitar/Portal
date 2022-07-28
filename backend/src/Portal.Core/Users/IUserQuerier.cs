using Portal.Core.Realms;

namespace Portal.Core.Users
{
  public interface IUserQuerier
  {
    Task<User?> GetAsync(string username, Realm? realm = null, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<User?> GetAsync(Guid id, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<PagedList<User>> GetPagedAsync(Guid? realmId = null, string? search = null,
      UserSort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      bool readOnly = false, CancellationToken cancellationToken = default);
  }
}
