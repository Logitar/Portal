using FluentValidation;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands
{
  internal class SignOutUserSessionsCommandHandler : IRequestHandler<SignOutUserSessionsCommand, IEnumerable<SessionModel>>
  {
    private readonly IRepository _repository;
    private readonly ISessionQuerier _sessionQuerier;
    private readonly ISessionRepository _sessionRepository;
    private readonly IValidator<Session> _sessionValidator;
    private readonly IUserContext _userContext;

    public SignOutUserSessionsCommandHandler(IRepository repository,
      ISessionQuerier sessionQuerier,
      ISessionRepository sessionRepository,
      IValidator<Session> sessionValidator,
      IUserContext userContext)
    {
      _repository = repository;
      _sessionQuerier = sessionQuerier;
      _sessionRepository = sessionRepository;
      _sessionValidator = sessionValidator;
      _userContext = userContext;
    }

    public async Task<IEnumerable<SessionModel>> Handle(SignOutUserSessionsCommand request, CancellationToken cancellationToken)
    {
      User user = await _repository.LoadAsync<User>(request.UserId, cancellationToken)
        ?? throw new EntityNotFoundException<User>(request.UserId);

      IEnumerable<Session> sessions = await _sessionRepository.LoadActiveByUserAsync(user, cancellationToken);

      foreach (Session session in sessions)
      {
        session.SignOut(_userContext.ActorId);
        _sessionValidator.ValidateAndThrow(session);
      }

      await _repository.SaveAsync(sessions, cancellationToken);

      return await _sessionQuerier.GetAsync(sessions.Select(x => x.Id), cancellationToken);
    }
  }
}
