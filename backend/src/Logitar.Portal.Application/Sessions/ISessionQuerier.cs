using Logitar.Portal.Contracts.Sessions.Models;
using Logitar.Portal.Domain;

namespace Logitar.Portal.Application.Sessions
{
  public interface ISessionQuerier
  {
    Task<SessionModel?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
  }
}
