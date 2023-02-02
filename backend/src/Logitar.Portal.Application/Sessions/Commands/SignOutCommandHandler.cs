using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands
{
  internal class SignOutCommandHandler : IRequestHandler<SignOutCommand, SessionModel>
  {
    private readonly IRepository _repository;
    private readonly ISessionQuerier _sessionQuerier;
    private readonly IUserContext _userContext;

    public SignOutCommandHandler(IRepository repository,
      ISessionQuerier sessionQuerier,
      IUserContext userContext)
    {
      _repository = repository;
      _sessionQuerier = sessionQuerier;
      _userContext = userContext;
    }

    public async Task<SessionModel> Handle(SignOutCommand request, CancellationToken cancellationToken)
    {
      Session session = await _repository.LoadAsync<Session>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<Session>(request.Id);

      session.SignOut(_userContext.ActorId);

      await _repository.SaveAsync(session, cancellationToken);

      return await _sessionQuerier.GetAsync(session.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The session 'Id={session.Id} could not be found.");
    }
  }
}
