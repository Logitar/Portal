using Portal.Core.Realms;

namespace Portal.Core.Users
{
  public interface IUserQuerier
  {
    Task<User?> GetAsync(string username, Realm? realm = null, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAsync(IEnumerable<string> usernames, Realm? realm = null, bool readOnly = false, CancellationToken cancelationToken = default);
    Task<User?> GetAsync(Guid id, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAsync(IEnumerable<Guid> ids, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByEmailAsync(string email, Realm? realm = null, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<PagedList<User>> GetPagedAsync(bool? isConfirmed = null, bool? isDisabled = null, string? realm = null, string? search = null,
      UserSort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      bool readOnly = false, CancellationToken cancellationToken = default);
  }
}
