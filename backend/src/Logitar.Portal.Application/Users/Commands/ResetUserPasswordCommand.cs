using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Application.Users.Validators;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record ResetUserPasswordCommand(Guid Id, ResetUserPasswordPayload Payload) : Activity, IRequest<UserModel?>
{
  public override IActivity Anonymize()
  {
    ResetUserPasswordCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}

internal class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, UserModel?>
{
  private readonly IPasswordManager _passwordManager;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public ResetUserPasswordCommandHandler(IPasswordManager passwordManager, IUserManager userManager, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _passwordManager = passwordManager;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<UserModel?> Handle(ResetUserPasswordCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = command.UserSettings;

    ResetUserPasswordPayload payload = command.Payload;
    new ResetUserPasswordValidator(command.UserSettings).ValidateAndThrow(payload);

    UserAggregate? user = await _userRepository.LoadAsync(command.Id, cancellationToken);
    if (user == null || user.TenantId != command.TenantId)
    {
      return null;
    }
    ActorId actorId = new(user.Id.Value);

    Password password = _passwordManager.ValidateAndCreate(payload.Password, userSettings.Password);
    user.ResetPassword(password, actorId);

    await _userManager.SaveAsync(user, userSettings, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(command.Realm, user, cancellationToken);
  }
}
