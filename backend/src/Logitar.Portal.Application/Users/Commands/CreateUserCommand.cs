using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Application.Users.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record CreateUserCommand(CreateUserPayload Payload) : Activity, IRequest<UserModel>
{
  public override IActivity Anonymize()
  {
    if (Payload.Password == null)
    {
      return base.Anonymize();
    }

    CreateUserCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}

internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserModel>
{
  private readonly IMediator _mediator;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;

  public CreateUserCommandHandler(IMediator mediator, IPasswordManager passwordManager, IUserManager userManager, IUserQuerier userQuerier)
  {
    _mediator = mediator;
    _passwordManager = passwordManager;
    _userManager = userManager;
    _userQuerier = userQuerier;
  }

  public async Task<UserModel> Handle(CreateUserCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = command.UserSettings;

    CreateUserPayload payload = command.Payload;
    new CreateUserValidator(userSettings).ValidateAndThrow(payload);

    ActorId actorId = command.ActorId;

    UniqueNameUnit uniqueName = new(userSettings.UniqueName, payload.UniqueName);
    UserAggregate user = new(uniqueName, command.TenantId, actorId)
    {
      FirstName = PersonNameUnit.TryCreate(payload.FirstName),
      MiddleName = PersonNameUnit.TryCreate(payload.MiddleName),
      LastName = PersonNameUnit.TryCreate(payload.LastName),
      Nickname = PersonNameUnit.TryCreate(payload.Nickname),
      Birthdate = payload.Birthdate,
      Gender = GenderUnit.TryCreate(payload.Gender),
      Locale = LocaleUnit.TryCreate(payload.Locale),
      TimeZone = TimeZoneUnit.TryCreate(payload.TimeZone),
      Picture = UrlUnit.TryCreate(payload.Picture),
      Profile = UrlUnit.TryCreate(payload.Profile),
      Website = UrlUnit.TryCreate(payload.Website)
    };

    if (payload.Password != null)
    {
      Password password = _passwordManager.ValidateAndCreate(payload.Password, userSettings.Password);
      user.SetPassword(password, actorId);
    }
    if (payload.IsDisabled)
    {
      user.Disable(actorId);
    }

    if (payload.Address != null)
    {
      AddressUnit address = payload.Address.ToAddressUnit();
      user.SetAddress(address, actorId);
    }
    if (payload.Email != null)
    {
      EmailUnit email = payload.Email.ToEmailUnit();
      user.SetEmail(email, actorId);
    }
    if (payload.Phone != null)
    {
      PhoneUnit phone = payload.Phone.ToPhoneUnit();
      user.SetPhone(phone, actorId);
    }

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      user.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }

    foreach (CustomIdentifier customIdentifier in payload.CustomIdentifiers)
    {
      user.SetCustomIdentifier(customIdentifier.Key, customIdentifier.Value, actorId);
    }

    IEnumerable<FoundRole> roles = await _mediator.Send(new FindRolesQuery(user.TenantId, payload.Roles, nameof(payload.Roles)), cancellationToken);
    foreach (FoundRole found in roles)
    {
      user.AddRole(found.Role, actorId);
    }

    user.Update(actorId);
    await _userManager.SaveAsync(user, userSettings, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(command.Realm, user, cancellationToken);
  }
}
