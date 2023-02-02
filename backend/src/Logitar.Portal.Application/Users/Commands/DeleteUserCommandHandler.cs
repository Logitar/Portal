using FluentValidation;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands
{
  internal class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
  {
    private readonly IRepository _repository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IValidator<Session> _sessionValidator;
    private readonly IUserContext _userContext;
    private readonly IUserValidator _userValidator;

    public DeleteUserCommandHandler(IRepository repository,
      ISessionRepository sessionRepository,
      IValidator<Session> sessionValidator,
      IUserContext userContext,
      IUserValidator userValidator)
    {
      _repository = repository;
      _sessionRepository = sessionRepository;
      _sessionValidator = sessionValidator;
      _userContext = userContext;
      _userValidator = userValidator;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
      User user = await _repository.LoadAsync<User>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<User>(request.Id);

      user.Delete(_userContext.ActorId);
      await _userValidator.ValidateAndThrowAsync(user, cancellationToken);

      IEnumerable<Session> sessions = await _sessionRepository.LoadActiveByUserAsync(user, cancellationToken);
      foreach (Session session in sessions)
      {
        session.Delete(_userContext.ActorId);
        _sessionValidator.ValidateAndThrow(session);
      }

      await _repository.SaveAsync(new AggregateRoot[] { user }.Concat(sessions), cancellationToken);

      return Unit.Value;
    }
  }
}
