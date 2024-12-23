using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Application.Users.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class ReplaceUserCommandHandler : IRequestHandler<ReplaceUserCommand, UserModel?>
{
  private readonly IMediator _mediator;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public ReplaceUserCommandHandler(IMediator mediator, IPasswordManager passwordManager,
    IUserManager userManager, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _mediator = mediator;
    _passwordManager = passwordManager;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<UserModel?> Handle(ReplaceUserCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = command.UserSettings;

    ReplaceUserPayload payload = command.Payload;
    new ReplaceUserValidator(userSettings).ValidateAndThrow(payload);

    UserAggregate? user = await _userRepository.LoadAsync(command.Id, cancellationToken);
    if (user == null || user.TenantId != command.TenantId)
    {
      return null;
    }
    UserAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _userRepository.LoadAsync(user.Id, command.Version.Value, cancellationToken);
    }

    ActorId actorId = command.ActorId;

    ReplaceAuthenticationInformation(userSettings, payload, user, reference, actorId);
    ReplaceContactInformation(payload, user, reference, actorId);
    ReplacePersonalInformation(payload, user, reference);
    ReplaceCustomAttributes(payload, user, reference);
    await ReplaceRolesAsync(payload, user, reference, actorId, cancellationToken);

    user.Update(actorId);
    await _userManager.SaveAsync(user, userSettings, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(command.Realm, user, cancellationToken);
  }

  private void ReplaceAuthenticationInformation(IUserSettings userSettings, ReplaceUserPayload payload, UserAggregate user, UserAggregate? reference, ActorId actorId)
  {
    UniqueName uniqueName = new(userSettings.UniqueName, payload.UniqueName);
    if (reference == null || uniqueName != reference.UniqueName)
    {
      user.SetUniqueName(uniqueName, actorId);
    }

    if (payload.Password != null)
    {
      Password password = _passwordManager.ValidateAndCreate(payload.Password, userSettings.Password);
      user.SetPassword(password, actorId);
    }

    if (reference == null || payload.IsDisabled != reference.IsDisabled)
    {
      if (payload.IsDisabled)
      {
        user.Disable(actorId);
      }
      else
      {
        user.Enable(actorId);
      }
    }
  }

  private static void ReplaceContactInformation(ReplaceUserPayload payload, UserAggregate user, UserAggregate? reference, ActorId actorId)
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

  private static void ReplacePersonalInformation(ReplaceUserPayload payload, UserAggregate user, UserAggregate? reference)
  {
    PersonNameUnit? firstName = PersonNameUnit.TryCreate(payload.FirstName);
    if (reference == null || firstName != reference.FirstName)
    {
      user.FirstName = firstName;
    }
    PersonNameUnit? middleName = PersonNameUnit.TryCreate(payload.MiddleName);
    if (reference == null || middleName != reference.MiddleName)
    {
      user.MiddleName = middleName;
    }
    PersonNameUnit? lastName = PersonNameUnit.TryCreate(payload.LastName);
    if (reference == null || lastName != reference.LastName)
    {
      user.LastName = lastName;
    }
    PersonNameUnit? nickname = PersonNameUnit.TryCreate(payload.Nickname);
    if (reference == null || nickname != reference.Nickname)
    {
      user.Nickname = nickname;
    }

    if (reference == null || payload.Birthdate != reference.Birthdate)
    {
      user.Birthdate = payload.Birthdate;
    }
    GenderUnit? gender = GenderUnit.TryCreate(payload.Gender);
    if (reference == null || gender != reference.Gender)
    {
      user.Gender = gender;
    }
    Locale? locale = Locale.TryCreate(payload.Locale);
    if (reference == null || locale != reference.Locale)
    {
      user.Locale = locale;
    }
    TimeZoneUnit? timeZone = TimeZoneUnit.TryCreate(payload.TimeZone);
    if (reference == null || timeZone != reference.TimeZone)
    {
      user.TimeZone = timeZone;
    }

    UrlUnit? picture = UrlUnit.TryCreate(payload.Picture);
    if (reference == null || picture != reference.Picture)
    {
      user.Picture = picture;
    }
    UrlUnit? profile = UrlUnit.TryCreate(payload.Profile);
    if (reference == null || profile != reference.Profile)
    {
      user.Profile = profile;
    }
    UrlUnit? website = UrlUnit.TryCreate(payload.Website);
    if (reference == null || website != reference.Website)
    {
      user.Website = website;
    }
  }

  private static void ReplaceCustomAttributes(ReplaceUserPayload payload, UserAggregate user, UserAggregate? reference)
  {
    HashSet<string> payloadKeys = new(capacity: payload.CustomAttributes.Count);

    IEnumerable<string> referenceKeys;
    if (reference == null)
    {
      referenceKeys = user.CustomAttributes.Keys;

      foreach (CustomAttribute customAttribute in payload.CustomAttributes)
      {
        payloadKeys.Add(customAttribute.Key.Trim());
        user.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
      }
    }
    else
    {
      referenceKeys = reference.CustomAttributes.Keys;

      foreach (CustomAttribute customAttribute in payload.CustomAttributes)
      {
        string key = customAttribute.Key.Trim();
        payloadKeys.Add(key);

        string value = customAttribute.Value.Trim();
        if (!reference.CustomAttributes.TryGetValue(key, out string? existingValue) || value != existingValue)
        {
          user.SetCustomAttribute(key, value);
        }
      }
    }

    foreach (string key in referenceKeys)
    {
      if (!payloadKeys.Contains(key))
      {
        user.RemoveCustomAttribute(key);
      }
    }
  }

  private async Task ReplaceRolesAsync(ReplaceUserPayload payload, UserAggregate user, UserAggregate? reference, ActorId actorId, CancellationToken cancellationToken)
  {
    IEnumerable<FoundRole> roles = await _mediator.Send(new FindRolesQuery(user.TenantId, payload.Roles, nameof(payload.Roles)), cancellationToken);
    HashSet<RoleId> roleIds = new(capacity: roles.Count());

    IEnumerable<RoleId> referenceRoles;
    if (reference == null)
    {
      referenceRoles = user.Roles;

      foreach (FoundRole found in roles)
      {
        roleIds.Add(found.Role.Id);
        user.AddRole(found.Role, actorId);
      }
    }
    else
    {
      referenceRoles = reference.Roles;

      foreach (FoundRole found in roles)
      {
        if (!reference.HasRole(found.Role))
        {
          roleIds.Add(found.Role.Id);
          user.AddRole(found.Role, actorId);
        }
      }
    }

    foreach (RoleId roleId in referenceRoles)
    {
      if (!roleIds.Contains(roleId))
      {
        Role role = new(roleId.AggregateId);
        user.RemoveRole(role, actorId);
      }
    }
  }
}
