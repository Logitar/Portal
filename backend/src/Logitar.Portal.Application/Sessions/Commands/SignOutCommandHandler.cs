using Logitar.Portal.Domain.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands
{
  internal class SignOutCommandHandler : IRequestHandler<SignOutCommand>
  {
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;

    public SignOutCommandHandler(IRepository repository, IUserContext userContext)
    {
      _repository = repository;
      _userContext = userContext;
    }

    public async Task<Unit> Handle(SignOutCommand request, CancellationToken cancellationToken)
    {
      Session session = await _repository.LoadAsync<Session>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<Session>(request.Id);

      session.SignOut(_userContext.ActorId);

      await _repository.SaveAsync(session, cancellationToken);

      return Unit.Value;
    }
  }
}
