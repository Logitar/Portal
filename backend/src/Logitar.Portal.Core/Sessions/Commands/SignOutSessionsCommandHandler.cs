using FluentValidation;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands
{
  internal class SignOutSessionsCommandHandler : IRequestHandler<SignOutSessionsCommand, IEnumerable<SessionModel>>
  {
    private readonly IRepository _repository;
    private readonly ISessionQuerier _sessionQuerier;
    private readonly IValidator<Session> _sessionValidator;
    private readonly IUserContext _userContext;

    public SignOutSessionsCommandHandler(IRepository repository,
      ISessionQuerier sessionQuerier,
      IValidator<Session> sessionValidator,
      IUserContext userContext)
    {
      _repository = repository;
      _sessionQuerier = sessionQuerier;
      _sessionValidator = sessionValidator;
      _userContext = userContext;
    }

    public async Task<IEnumerable<SessionModel>> Handle(SignOutSessionsCommand request, CancellationToken cancellationToken)
    {
      if (await _repository.LoadAsync<User>(new AggregateId(request.UserId), cancellationToken) == null)
      {
        throw EntityNotFoundException.Typed<User>(request.UserId, nameof(request.UserId));
      }

      IEnumerable<AggregateId> ids = (await _sessionQuerier.GetPagedAsync(isActive: true, userId: request.UserId, cancellationToken: cancellationToken))
        .Items
        .Select(s => new AggregateId(s.Id));
      IEnumerable<Session> sessions = await _repository.LoadAsync<Session>(ids, cancellationToken);

      foreach (Session session in sessions)
      {
        session.SignOut(_userContext.ActorId);
        _sessionValidator.ValidateAndThrow(session);
      }

      await _repository.SaveAsync(sessions, cancellationToken);

      return (await _sessionQuerier.GetPagedAsync(isActive: true, userId: request.UserId, cancellationToken: cancellationToken)).Items;
    }
  }
}
