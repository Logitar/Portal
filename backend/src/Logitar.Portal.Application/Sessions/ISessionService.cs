using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Application.Sessions;

public interface ISessionService
{
  Task<SessionModel> CreateAsync(CreateSessionPayload payload, CancellationToken cancellationToken = default);
  Task<SessionModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SessionModel> RenewAsync(RenewSessionPayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<SessionModel>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken = default);
  Task<SessionModel> SignInAsync(SignInSessionPayload payload, CancellationToken cancellationToken = default);
  Task<SessionModel?> SignOutAsync(Guid id, CancellationToken cancellationToken = default);
}
