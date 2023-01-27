using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;

namespace Logitar.Portal.Core.Users
{
  public interface IUserService
  {
    Task<UserModel> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken = default);
    Task<UserModel> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<UserModel> DisableAsync(string id, CancellationToken cancellationToken = default);
    Task<UserModel> EnableAsync(string id, CancellationToken cancellationToken = default);
    Task<UserModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<UserModel>> GetAsync(bool? isConfirmed = null, bool? isDisabled = null, string? realm = null, string? search = null,
      UserSort? sort = null, bool isDescending = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
    Task<UserModel> UpdateAsync(string id, UpdateUserPayload payload, CancellationToken cancellationToken = default);
  }
}
