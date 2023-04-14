using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Core.Sessions.Commands;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal class DeleteUserHandler : IRequestHandler<DeleteUser, User>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMediator _mediator;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public DeleteUserHandler(IApplicationContext applicationContext,
    IMediator mediator,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
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

    user.Delete(_applicationContext.ActorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return output;
  }
}
