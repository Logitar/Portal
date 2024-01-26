using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Users.Validators;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public UpdateUserCommandHandler(IApplicationContext applicationContext, IPasswordManager passwordManager,
    IUserManager userManager, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _passwordManager = passwordManager;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = _applicationContext.UserSettings;

    UpdateUserPayload payload = command.Payload;
    new UpdateUserValidator(userSettings).ValidateAndThrow(payload);

    UserId userId = new(command.Id);
    UserAggregate? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null)
    {
      return null;
    }
    // TODO(fpion): ensure User is in current Realm

    ActorId actorId = _applicationContext.ActorId;

    SetAuthenticationInformation(payload, user, actorId, userSettings);
    SetContactInformation(payload, user, actorId);
    user.Update(actorId);

    await _userManager.SaveAsync(user, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }

  private void SetAuthenticationInformation(UpdateUserPayload payload, UserAggregate user, ActorId actorId, IUserSettings userSettings)
  {
    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueNameUnit uniqueName = new(userSettings.UniqueName, payload.UniqueName);
      user.SetUniqueName(uniqueName, actorId);
    }

    if (payload.Password != null)
    {
      Password newPassword = _passwordManager.ValidateAndCreate(payload.Password.NewPassword);
      if (payload.Password.CurrentPassword == null)
      {
        user.SetPassword(newPassword, actorId);
      }
      else
      {
        user.ChangePassword(payload.Password.CurrentPassword, newPassword, actorId);
      }
    }

    if (payload.IsDisabled.HasValue)
    {
      if (payload.IsDisabled.Value)
      {
        user.Disable(actorId);
      }
      else
      {
        user.Enable(actorId);
      }
    }
  }

  private static void SetContactInformation(UpdateUserPayload payload, UserAggregate user, ActorId actorId)
  {
    if (payload.Address != null)
    {
      AddressUnit? address = payload.Address.Value?.ToAddressUnit();
      user.SetAddress(address, actorId);
    }
    if (payload.Email != null)
    {
      EmailUnit? email = payload.Email.Value?.ToEmailUnit();
      user.SetEmail(email, actorId);
    }
    if (payload.Phone != null)
    {
      PhoneUnit? phone = payload.Phone.Value?.ToPhoneUnit();
      user.SetPhone(phone, actorId);
    }
  }
}
