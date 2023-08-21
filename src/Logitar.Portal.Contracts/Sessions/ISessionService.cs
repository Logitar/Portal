namespace Logitar.Portal.Contracts.Sessions;

public interface ISessionService
{
  Task<Session> CreateAsync(CreateSessionPayload payload, CancellationToken cancellationToken = default);
  Task<Session?> ReadAsync(string id, CancellationToken cancellationToken = default);
  Task<Session> RenewAsync(RenewSessionPayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<Session>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken = default);
  Task<Session> SignInAsync(SignInPayload payload, CancellationToken cancellationToken = default);
  Task<Session?> SignOutAsync(string id, CancellationToken cancellationToken = default);
}
