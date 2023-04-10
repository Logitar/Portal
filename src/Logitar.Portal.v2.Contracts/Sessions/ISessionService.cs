namespace Logitar.Portal.v2.Contracts.Sessions;

public interface ISessionService
{
  Task<Session> RefreshAsync(RefreshInput input, CancellationToken cancellationToken = default);
  Task<Session> SignInAsync(SignInInput input, CancellationToken cancellationToken = default);
}
