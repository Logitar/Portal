using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Sessions;

namespace Logitar.Portal.Application.Sessions;

public interface ISessionQuerier
{
  Task<Session> ReadAsync(SessionAggregate session, CancellationToken cancellationToken = default);
}
