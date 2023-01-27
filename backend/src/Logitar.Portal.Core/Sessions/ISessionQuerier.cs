using Logitar.Portal.Core.Sessions.Models;

namespace Logitar.Portal.Core.Sessions
{
  public interface ISessionQuerier
  {
    Task<SessionModel?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
  }
}
