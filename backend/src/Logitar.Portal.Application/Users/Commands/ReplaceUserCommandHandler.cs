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

internal class ReplaceUserCommandHandler : IRequestHandler<ReplaceUserCommand, User?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public ReplaceUserCommandHandler(IApplicationContext applicationContext, IPasswordManager passwordManager,
    IUserManager userManager, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _passwordManager = passwordManager;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(ReplaceUserCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = _applicationContext.UserSettings;

    ReplaceUserPayload payload = command.Payload;
    new ReplaceUserValidator(userSettings).ValidateAndThrow(payload);

    UserId userId = new(command.Id);
    UserAggregate? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null)
    {
      return null;
    }
    // TODO(fpion): ensure User is in current Realm

    UserAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _userRepository.LoadAsync(userId, command.Version.Value, cancellationToken);
    }

    ActorId actorId = _applicationContext.ActorId;

    SetAuthenticationInformation(payload, user, reference, actorId, userSettings);
    SetContactInformation(payload, user, reference, actorId);
    user.Update(actorId);

    await _userManager.SaveAsync(user, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }

  private void SetAuthenticationInformation(ReplaceUserPayload payload, UserAggregate user, UserAggregate? reference, ActorId actorId, IUserSettings userSettings)
  {
    UniqueNameUnit uniqueName = new(userSettings.UniqueName, payload.UniqueName);
    if (reference == null || uniqueName != reference.UniqueName)
    {
      user.SetUniqueName(uniqueName, actorId);
    }

    if (payload.Password != null)
    {
      Password password = _passwordManager.ValidateAndCreate(payload.Password);
      user.SetPassword(password, actorId);
    }

    if (payload.IsDisabled)
    {
      user.Disable(actorId);
    }
    else
    {
      user.Enable(actorId);
    }
  }

  private static void SetContactInformation(ReplaceUserPayload payload, UserAggregate user, UserAggregate? reference, ActorId actorId)
  {
    AddressUnit? address = payload.Address?.ToAddressUnit();
    if (reference == null || address != reference.Address)
    {
      user.SetAddress(address, actorId);
    }
    EmailUnit? email = payload.Email?.ToEmailUnit();
    if (reference == null || email != reference.Email)
    {
      user.SetEmail(email, actorId);
    }
    PhoneUnit? phone = payload.Phone?.ToPhoneUnit();
    if (reference == null || phone != reference.Phone)
    {
      user.SetPhone(phone, actorId);
    }
  }
}
