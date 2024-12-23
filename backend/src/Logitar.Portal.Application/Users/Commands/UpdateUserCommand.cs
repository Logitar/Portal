using FluentValidation;
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

public record UpdateUserCommand(Guid Id, UpdateUserPayload Payload) : Activity, IRequest<UserModel?>
{
  public override IActivity Anonymize()
  {
    if (Payload.Password == null)
    {
      return base.Anonymize();
    }

    UpdateUserCommand command = this.DeepClone();
    if (command.Payload.Password != null)
    {
      if (command.Payload.Password.Current != null)
      {
        command.Payload.Password.Current = command.Payload.Password.Current.Mask();
      }
      command.Payload.Password.New = command.Payload.Password.New.Mask();
    }
    return command;
  }
}

internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserModel?>
{
  private readonly IAddressHelper _addressHelper;
  private readonly IMediator _mediator;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public UpdateUserCommandHandler(
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

  public async Task<UserModel?> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = command.UserSettings;

    UpdateUserPayload payload = command.Payload;
    new UpdateUserValidator(_addressHelper, userSettings).ValidateAndThrow(payload);

    UserId userId = new(command.TenantId, new EntityId(command.Id));
    User? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null || user.TenantId != command.TenantId)
    {
      return null;
    }

    ActorId actorId = command.ActorId;

    UpdateAuthenticationInformation(userSettings, payload, user, actorId);
    UpdateContactInformation(payload, user, actorId);
    UpdatePersonalInformation(payload, user);

    foreach (CustomAttributeModification customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      if (string.IsNullOrWhiteSpace(customAttribute.Value))
      {
        user.RemoveCustomAttribute(key);
      }
      else
      {
        user.SetCustomAttribute(key, customAttribute.Value);
      }
    }

    IEnumerable<FoundRole> roles = await _mediator.Send(new FindRolesQuery(user.TenantId, payload.Roles, nameof(payload.Roles)), cancellationToken);
    foreach (FoundRole found in roles)
    {
      switch (found.Action)
      {
        case CollectionAction.Add:
          user.AddRole(found.Role, actorId);
          break;
        case CollectionAction.Remove:
          user.RemoveRole(found.Role, actorId);
          break;
      }
    }

    user.Update(actorId);
    await _userManager.SaveAsync(user, userSettings, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(command.Realm, user, cancellationToken);
  }

  private void UpdateAuthenticationInformation(IUserSettings userSettings, UpdateUserPayload payload, User user, ActorId actorId)
  {
    UniqueName? uniqueName = UniqueName.TryCreate(payload.UniqueName, userSettings.UniqueName);
    if (uniqueName != null)
    {
      user.SetUniqueName(uniqueName, actorId);
    }

    if (payload.Password != null)
    {
      Password newPassword = _passwordManager.ValidateAndCreate(payload.Password.New, userSettings.Password);
      if (payload.Password.Current == null)
      {
        user.SetPassword(newPassword, actorId);
      }
      else
      {
        user.ChangePassword(payload.Password.Current, newPassword, new ActorId(user.Id.Value));
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

  private void UpdateContactInformation(UpdateUserPayload payload, User user, ActorId actorId)
  {
    if (payload.Address != null)
    {
      Address? address = payload.Address.Value?.ToAddress(_addressHelper);
      user.SetAddress(address, actorId);
    }

    if (payload.Email != null)
    {
      Email? email = payload.Email.Value?.ToEmail();
      user.SetEmail(email, actorId);
    }

    if (payload.Phone != null)
    {
      Phone? phone = payload.Phone.Value?.ToPhone();
      user.SetPhone(phone, actorId);
    }
  }

  private static void UpdatePersonalInformation(UpdateUserPayload payload, User user)
  {
    if (payload.FirstName != null)
    {
      user.FirstName = PersonName.TryCreate(payload.FirstName.Value);
    }
    if (payload.MiddleName != null)
    {
      user.MiddleName = PersonName.TryCreate(payload.MiddleName.Value);
    }
    if (payload.LastName != null)
    {
      user.LastName = PersonName.TryCreate(payload.LastName.Value);
    }
    if (payload.Nickname != null)
    {
      user.Nickname = PersonName.TryCreate(payload.Nickname.Value);
    }

    if (payload.Birthdate != null)
    {
      user.Birthdate = payload.Birthdate.Value;
    }
    if (payload.Gender != null)
    {
      user.Gender = Gender.TryCreate(payload.Gender.Value);
    }
    if (payload.Locale != null)
    {
      user.Locale = Locale.TryCreate(payload.Locale.Value);
    }
    if (payload.TimeZone != null)
    {
      user.TimeZone = TimeZone.TryCreate(payload.TimeZone.Value);
    }

    if (payload.Picture != null)
    {
      user.Picture = Url.TryCreate(payload.Picture.Value);
    }
    if (payload.Profile != null)
    {
      user.Profile = Url.TryCreate(payload.Profile.Value);
    }
    if (payload.Website != null)
    {
      user.Website = Url.TryCreate(payload.Website.Value);
    }
  }
}
