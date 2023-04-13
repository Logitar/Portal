using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Core.Sessions.Commands;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal class DeleteUserHandler : IRequestHandler<DeleteUser, User>
{
  private readonly ICurrentActor _currentActor;
  private readonly IMediator _mediator;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public DeleteUserHandler(ICurrentActor currentActor,
    IMediator mediator,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _currentActor = currentActor;
    _mediator = mediator;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(DeleteUser request, CancellationToken cancellationToken)
  {
    UserAggregate user = await _userRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(request.Id);
    User output = await _userQuerier.GetAsync(user, cancellationToken);

    await _mediator.Send(new DeleteSessions(user), cancellationToken);

    user.Delete(_currentActor.Id);

    await _userRepository.SaveAsync(user, cancellationToken);

    return output;
  }
}
