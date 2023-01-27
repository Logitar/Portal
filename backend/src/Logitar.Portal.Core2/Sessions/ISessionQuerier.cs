using Logitar.Portal.Core2.Sessions.Models;

namespace Logitar.Portal.Core2.Sessions
{
  public interface ISessionQuerier
  {
    Task<SessionModel?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
  }
}
