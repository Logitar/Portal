using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Users.Validators;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, User?>
{
  private readonly IPasswordManager _passwordManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public ResetUserPasswordCommandHandler(IPasswordManager passwordManager, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _passwordManager = passwordManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(ResetUserPasswordCommand command, CancellationToken cancellationToken)
  {
    ResetUserPasswordPayload payload = command.Payload;
    new ResetUserPasswordValidator(command.UserSettings).ValidateAndThrow(payload);

    UserAggregate? user = await _userRepository.LoadAsync(command.Id, cancellationToken);
    if (user == null || user.TenantId != command.TenantId)
    {
      return null;
    }
    ActorId actorId = new(user.Id.Value);

    Password password = _passwordManager.ValidateAndCreate(payload.Password);
    user.ResetPassword(password, actorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(command.Realm, user, cancellationToken);
  }
}
