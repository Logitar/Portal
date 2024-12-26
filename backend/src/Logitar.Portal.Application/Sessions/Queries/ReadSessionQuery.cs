using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Queries;

public record ReadSessionQuery(Guid Id) : Activity, IRequest<SessionModel?>;

internal class ReadSessionQueryHandler : IRequestHandler<ReadSessionQuery, SessionModel?>
{
  private readonly ISessionQuerier _sessionQuerier;

  public ReadSessionQueryHandler(ISessionQuerier sessionQuerier)
  {
    _sessionQuerier = sessionQuerier;
  }

  public async Task<SessionModel?> Handle(ReadSessionQuery query, CancellationToken cancellationToken)
  {
    return await _sessionQuerier.ReadAsync(query.Realm, query.Id, cancellationToken);
  }
}
