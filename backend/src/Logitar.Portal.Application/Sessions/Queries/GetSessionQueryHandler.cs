using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Queries
{
  internal class GetSessionQueryHandler : IRequestHandler<GetSessionQuery, SessionModel?>
  {
    private readonly ISessionQuerier _sessionQuerier;

    public GetSessionQueryHandler(ISessionQuerier sessionQuerier)
    {
      _sessionQuerier = sessionQuerier;
    }

    public async Task<SessionModel?> Handle(GetSessionQuery request, CancellationToken cancellationToken)
    {
      return await _sessionQuerier.GetAsync(request.Id, cancellationToken);
    }
  }
}
