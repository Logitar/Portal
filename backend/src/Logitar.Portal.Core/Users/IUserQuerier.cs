using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Users.Models;

namespace Logitar.Portal.Core.Users
{
  public interface IUserQuerier
  {
    Task<UserModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<UserModel?> GetAsync(string username, string? realm = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserModel>> GetByEmailAsync(string email, string? realm = null, CancellationToken cancellationToken = default);
    Task<ListModel<UserModel>> GetPagedAsync(bool? isConfirmed = null, bool? isDisabled = null, string? realm = null, string? search = null,
      UserSort? sort = null, bool isDescending = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
  }
}
