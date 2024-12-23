using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Sessions;

public interface ISessionClient
{
  Task<SessionModel> CreateAsync(CreateSessionPayload payload, IRequestContext? context = null);
  Task<SessionModel?> ReadAsync(Guid id, IRequestContext? context = null);
  Task<SessionModel> RenewAsync(RenewSessionPayload payload, IRequestContext? context = null);
  Task<SearchResults<SessionModel>> SearchAsync(SearchSessionsPayload payload, IRequestContext? context = null);
  Task<SessionModel> SignInAsync(SignInSessionPayload payload, IRequestContext? context = null);
  Task<SessionModel?> SignOutAsync(Guid id, IRequestContext? context = null);
}
