using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Core.Users;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands;

internal class RefreshHandler : IRequestHandler<Refresh, Session>
{
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;

  public RefreshHandler(ISessionQuerier sessionQuerier, ISessionRepository sessionRepository)
  {
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
  }

  public async Task<Session> Handle(Refresh request, CancellationToken cancellationToken)
  {
    RefreshInput input = request.Input;

    RefreshToken refreshToken;
    try
    {
      refreshToken = RefreshToken.Parse(input.RefreshToken);
    }
    catch (Exception innerException)
    {
      throw new InvalidCredentialsException("The refresh token could not be parsed.", innerException);
    }

    SessionAggregate session = await _sessionRepository.LoadAsync(refreshToken.Id, cancellationToken)
      ?? throw new InvalidCredentialsException($"The session aggregate '{refreshToken.Id}' could not be found.");
    Session output = await _sessionQuerier.GetAsync(session, cancellationToken);

    session.Refresh(refreshToken.Key, input.IpAddress, input.AdditionalInformation,
      input.CustomAttributes?.ToDictionary());

    await _sessionRepository.SaveAsync(session, cancellationToken);

    output.RefreshToken = session.RefreshToken?.ToString();

    return output;
  }
}
