﻿using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Application.Users.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;
using TimeZone = Logitar.Identity.Core.TimeZone;

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
  private readonly IAddressHelper _addressHelper;
  private readonly IMediator _mediator;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public CreateUserCommandHandler(
    IAddressHelper addressHelper,
    IMediator mediator,
    IPasswordManager passwordManager,
    IUserManager userManager,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _addressHelper = addressHelper;
    _mediator = mediator;
    _passwordManager = passwordManager;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<UserModel> Handle(CreateUserCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = command.UserSettings;

    CreateUserPayload payload = command.Payload;
    new CreateUserValidator(userSettings, _addressHelper).ValidateAndThrow(payload);

    UserId userId = UserId.NewId(command.TenantId);
    User? user;
    if (payload.Id.HasValue)
    {
      userId = new(command.TenantId, new EntityId(payload.Id.Value));
      user = await _userRepository.LoadAsync(userId, cancellationToken);
      if (user != null)
      {
        throw new IdAlreadyUsedException(payload.Id.Value, nameof(payload.Id));
      }
    }

    ActorId actorId = command.ActorId;

    UniqueName uniqueName = new(userSettings.UniqueName, payload.UniqueName);
    user = new(uniqueName, actorId, userId)
    {
      FirstName = PersonName.TryCreate(payload.FirstName),
      MiddleName = PersonName.TryCreate(payload.MiddleName),
      LastName = PersonName.TryCreate(payload.LastName),
      Nickname = PersonName.TryCreate(payload.Nickname),
      Birthdate = payload.Birthdate,
      Gender = Gender.TryCreate(payload.Gender),
      Locale = Locale.TryCreate(payload.Locale),
      TimeZone = TimeZone.TryCreate(payload.TimeZone),
      Picture = Url.TryCreate(payload.Picture),
      Profile = Url.TryCreate(payload.Profile),
      Website = Url.TryCreate(payload.Website)
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
      Address address = payload.Address.ToAddress(_addressHelper);
      user.SetAddress(address, actorId);
    }
    if (payload.Email != null)
    {
      Email email = payload.Email.ToEmail();
      user.SetEmail(email, actorId);
    }
    if (payload.Phone != null)
    {
      Phone phone = payload.Phone.ToPhone();
      user.SetPhone(phone, actorId);
    }

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      user.SetCustomAttribute(key, customAttribute.Value);
    }

    foreach (CustomIdentifierModel customIdentifier in payload.CustomIdentifiers)
    {
      Identifier key = new(customIdentifier.Key);
      CustomIdentifier value = new(customIdentifier.Value);
      user.SetCustomIdentifier(key, value, actorId);
    }

    IReadOnlyCollection<FoundRole> roles = await _mediator.Send(new FindRolesQuery(user.TenantId, payload.Roles, nameof(payload.Roles)), cancellationToken);
    foreach (FoundRole found in roles)
    {
      user.AddRole(found.Role, actorId);
    }

    user.Update(actorId);
    await _userManager.SaveAsync(user, userSettings, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(command.Realm, user, cancellationToken);
  }
}
