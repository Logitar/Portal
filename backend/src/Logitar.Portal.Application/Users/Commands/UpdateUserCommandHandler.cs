using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Application.Users.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User?>
{
  private readonly IMediator _mediator;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public UpdateUserCommandHandler(IMediator mediator,
    IPasswordManager passwordManager, IUserManager userManager, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _mediator = mediator;
    _passwordManager = passwordManager;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = command.UserSettings;

    UpdateUserPayload payload = command.Payload;
    new UpdateUserValidator(userSettings).ValidateAndThrow(payload);

    UserAggregate? user = await _userRepository.LoadAsync(command.Id, cancellationToken);
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
      if (string.IsNullOrWhiteSpace(customAttribute.Value))
      {
        user.RemoveCustomAttribute(customAttribute.Key);
      }
      else
      {
        user.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
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
    await _userManager.SaveAsync(user, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(command.Realm, user, cancellationToken);
  }

  private void UpdateAuthenticationInformation(IUserSettings userSettings, UpdateUserPayload payload, UserAggregate user, ActorId actorId)
  {
    UniqueNameUnit? uniqueName = UniqueNameUnit.TryCreate(userSettings.UniqueName, payload.UniqueName);
    if (uniqueName != null)
    {
      user.SetUniqueName(uniqueName, actorId);
    }

    if (payload.Password != null)
    {
      Password newPassword = _passwordManager.ValidateAndCreate(payload.Password.New);
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

  private static void UpdateContactInformation(UpdateUserPayload payload, UserAggregate user, ActorId actorId)
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

  private static void UpdatePersonalInformation(UpdateUserPayload payload, UserAggregate user)
  {
    if (payload.FirstName != null)
    {
      user.FirstName = PersonNameUnit.TryCreate(payload.FirstName.Value);
    }
    if (payload.MiddleName != null)
    {
      user.MiddleName = PersonNameUnit.TryCreate(payload.MiddleName.Value);
    }
    if (payload.LastName != null)
    {
      user.LastName = PersonNameUnit.TryCreate(payload.LastName.Value);
    }
    if (payload.Nickname != null)
    {
      user.Nickname = PersonNameUnit.TryCreate(payload.Nickname.Value);
    }

    if (payload.Birthdate != null)
    {
      user.Birthdate = payload.Birthdate.Value;
    }
    if (payload.Gender != null)
    {
      user.Gender = GenderUnit.TryCreate(payload.Gender.Value);
    }
    if (payload.Locale != null)
    {
      user.Locale = LocaleUnit.TryCreate(payload.Locale.Value);
    }
    if (payload.TimeZone != null)
    {
      user.TimeZone = TimeZoneUnit.TryCreate(payload.TimeZone.Value);
    }

    if (payload.Picture != null)
    {
      user.Picture = UrlUnit.TryCreate(payload.Picture.Value);
    }
    if (payload.Profile != null)
    {
      user.Profile = UrlUnit.TryCreate(payload.Profile.Value);
    }
    if (payload.Website != null)
    {
      user.Website = UrlUnit.TryCreate(payload.Website.Value);
    }
  }
}
