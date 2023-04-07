namespace Logitar.Portal.v2.Contracts.Users;

public interface IUserService
{
  Task<User> ChangePasswordAsync(Guid id, ChangePasswordInput input, CancellationToken cancellationToken = default);
  Task<User> CreateAsync(CreateUserInput input, CancellationToken cancellationToken = default);
  //Task<UserModel> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  //Task<UserModel> DisableAsync(Guid id, CancellationToken cancellationToken = default);
  //Task<UserModel> EnableAsync(Guid id, CancellationToken cancellationToken = default);
  //Task<UserModel> GetAsync(Guid id, CancellationToken cancellationToken = default);
  //Task<ListModel<UserSummary>> GetAsync(bool? isConfirmed = null, bool? isDisabled = null, string? realm = null, string? search = null,
  //  UserSort? sort = null, bool desc = false,
  //  int? index = null, int? count = null,
  //  CancellationToken cancellationToken = default);
  // TODO(fpion): SetExternalIdentifier
  //Task<UserModel> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken = default);
  // TODO(fpion): VerifyAddress
  // TODO(fpion): VerifyEmail
  // TODO(fpion): VerifyPhone
}
