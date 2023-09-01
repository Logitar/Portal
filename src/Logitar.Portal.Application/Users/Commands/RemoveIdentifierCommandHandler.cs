using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class RemoveIdentifierCommandHandler : IRequestHandler<RemoveIdentifierCommand, User?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public RemoveIdentifierCommandHandler(IApplicationContext applicationContext, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(RemoveIdentifierCommand command, CancellationToken cancellationToken)
  {
    UserAggregate? user = await _userRepository.LoadAsync(command.Id, cancellationToken);
    if (user == null)
    {
      return null;
    }

    user.RemoveIdentifier(command.Key, _applicationContext.ActorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
