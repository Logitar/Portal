using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;

namespace Logitar.Portal.Core.Users
{
  public interface IUserService
  {
    Task<UserModel> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken = default);
    Task<UserModel> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserModel> DisableAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserModel> EnableAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserModel?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ListModel<UserModel>> GetAsync(bool? isConfirmed = null, bool? isDisabled = null, string? realm = null, string? search = null,
      UserSort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
    Task<UserModel> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken = default);
  }
}
