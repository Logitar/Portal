using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands
{
  internal class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
  {
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;

    public DeleteUserCommandHandler(IRepository repository, IUserContext userContext)
    {
      _repository = repository;
      _userContext = userContext;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
      User user = await _repository.LoadAsync<User>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<User>(request.Id);

      IEnumerable<Session> sessions = await _repository.LoadSessionsByUserAsync(user, cancellationToken);
      foreach (Session session in sessions)
      {
        session.Delete(_userContext.ActorId);
      }
      await _repository.SaveAsync(sessions, cancellationToken);

      user.Delete(_userContext.ActorId);
      await _repository.SaveAsync(user, cancellationToken);

      return Unit.Value;
    }
  }
}
