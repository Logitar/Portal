using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Sessions;

public interface ISessionClient
{
  Task<Session> CreateAsync(CreateSessionPayload payload, IRequestContext? context = null);
  Task<Session?> ReadAsync(Guid id, IRequestContext? context = null);
  Task<Session> RenewAsync(RenewSessionPayload payload, IRequestContext? context = null);
  Task<SearchResults<Session>> SearchAsync(SearchSessionsPayload payload, IRequestContext? context = null);
  Task<Session> SignInAsync(SignInSessionPayload payload, IRequestContext? context = null);
  Task<Session?> SignOutAsync(Guid id, IRequestContext? context = null);
}
