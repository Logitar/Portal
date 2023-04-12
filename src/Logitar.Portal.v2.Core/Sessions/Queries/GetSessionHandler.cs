using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.v2.Core.Sessions.Queries;

internal class GetSessionHandler : IRequestHandler<GetSession, Session?>
{
  private readonly ISessionQuerier _sessionQuerier;

  public GetSessionHandler(ISessionQuerier sessionQuerier)
  {
    _sessionQuerier = sessionQuerier;
  }

  public async Task<Session?> Handle(GetSession request, CancellationToken cancellationToken)
  {
    List<Session> sessions = new(capacity: 1);

    if (request.Id.HasValue)
    {
      sessions.AddIfNotNull(await _sessionQuerier.GetAsync(request.Id.Value, cancellationToken));
    }

    if (sessions.Count > 1)
    {
      throw new TooManyResultsException();
    }

    return sessions.SingleOrDefault();
  }
}
