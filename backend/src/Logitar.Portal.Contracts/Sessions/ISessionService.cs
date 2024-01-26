namespace Logitar.Portal.Contracts.Sessions;

public interface ISessionService
{
  Task<Session> RenewAsync(RenewSessionPayload payload, CancellationToken cancellationToken = default);
  Task<Session> SignInAsync(SignInSessionPayload payload, CancellationToken cancellationToken = default);
  Task<Session?> SignOutAsync(string id, CancellationToken cancellationToken = default);
}
