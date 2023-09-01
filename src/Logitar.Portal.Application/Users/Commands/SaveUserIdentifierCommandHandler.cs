using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class SaveUserIdentifierCommandHandler : IRequestHandler<SaveUserIdentifierCommand, User?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public SaveUserIdentifierCommandHandler(IApplicationContext applicationContext, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(SaveUserIdentifierCommand command, CancellationToken cancellationToken)
  {
    UserAggregate? user = await _userRepository.LoadAsync(command.Id, cancellationToken);
    if (user == null)
    {
      return null;
    }

    SaveIdentifierPayload payload = command.Payload;

    UserAggregate? other = await _userRepository.LoadAsync(user.TenantId, payload.Key, payload.Value, cancellationToken);
    if (other?.Equals(user) == false)
    {
      string propertyName = string.Join(',', nameof(payload.Key), nameof(payload.Value));
      throw new IdentifierAlreadyUsedException<UserAggregate>(user.TenantId, payload.Key, payload.Value, propertyName);
    }

    user.SetIdentifier(payload.Key, payload.Value, _applicationContext.ActorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
