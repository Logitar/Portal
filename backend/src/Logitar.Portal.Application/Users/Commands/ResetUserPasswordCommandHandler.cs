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
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public ResetUserPasswordCommandHandler(IApplicationContext applicationContext,
    IPasswordManager passwordManager, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _passwordManager = passwordManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(ResetUserPasswordCommand command, CancellationToken cancellationToken)
  {
    ResetUserPasswordPayload payload = command.Payload;
    new ResetUserPasswordValidator(_applicationContext.UserSettings).ValidateAndThrow(payload);

    UserAggregate? user = await _userRepository.LoadAsync(command.Id, cancellationToken);
    if (user == null || user.TenantId != _applicationContext.TenantId)
    {
      return null;
    }
    ActorId actorId = new(user.Id.Value);

    Password password = _passwordManager.ValidateAndCreate(payload.Password);
    user.ResetPassword(password, actorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
