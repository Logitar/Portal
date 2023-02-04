using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain;

namespace Logitar.Portal.Application.Users
{
  public interface IUserQuerier
  {
    Task<UserModel?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
    Task<UserModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<UserModel>> GetPagedAsync(bool? isConfirmed = null, bool? isDisabled = null, string? realm = null, string? search = null,
      UserSort? sort = null, bool isDescending = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
  }
}
