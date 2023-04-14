using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal class DisableUserHandler : IRequestHandler<DisableUser, User>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public DisableUserHandler(IApplicationContext applicationContext,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(DisableUser request, CancellationToken cancellationToken)
  {
    UserAggregate user = await _userRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(request.Id);

    user.Disable(_applicationContext.ActorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user, cancellationToken);
  }
}
