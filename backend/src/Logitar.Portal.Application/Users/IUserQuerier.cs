using Logitar.Identity.Contracts.Users;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users;

public interface IUserQuerier
{
  Task<UserModel> ReadAsync(RealmModel? realm, User session, CancellationToken cancellationToken = default);
  Task<UserModel?> ReadAsync(RealmModel? realm, UserId id, CancellationToken cancellationToken = default);
  Task<UserModel?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken = default);
  Task<UserModel?> ReadAsync(RealmModel? realm, string uniqueName, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<UserModel>> ReadAsync(RealmModel? realm, IEmail email, CancellationToken cancellationToken = default);
  Task<UserModel?> ReadAsync(RealmModel? realm, CustomIdentifierModel identifier, CancellationToken cancellationToken = default);
  Task<SearchResults<UserModel>> SearchAsync(RealmModel? realm, SearchUsersPayload payload, CancellationToken cancellationToken = default);
}
