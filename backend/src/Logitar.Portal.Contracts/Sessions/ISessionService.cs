namespace Logitar.Portal.Contracts.Sessions;

public interface ISessionService
{
  Task<Session> SignInAsync(SignInSessionPayload payload, CancellationToken cancellationToken = default);
}
