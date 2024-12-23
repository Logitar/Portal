using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Roles;
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

internal record ReplaceUserCommand(Guid Id, ReplaceUserPayload Payload, long? Version) : Activity, IRequest<UserModel?>
{
  public override IActivity Anonymize()
  {
    if (Payload.Password == null)
    {
      return base.Anonymize();
    }

    ReplaceUserCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}

internal class ReplaceUserCommandHandler : IRequestHandler<ReplaceUserCommand, UserModel?>
{
  private readonly IAddressHelper _addressHelper;
  private readonly IMediator _mediator;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public ReplaceUserCommandHandler(
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

  public async Task<UserModel?> Handle(ReplaceUserCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = command.UserSettings;

    ReplaceUserPayload payload = command.Payload;
    new ReplaceUserValidator(_addressHelper, userSettings).ValidateAndThrow(payload);

    UserId userId = new(command.TenantId, new EntityId(command.Id));
    User? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null || user.TenantId != command.TenantId)
    {
      return null;
    }
    User? reference = null;
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

  private void ReplaceAuthenticationInformation(IUserSettings userSettings, ReplaceUserPayload payload, User user, User? reference, ActorId actorId)
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

  private void ReplaceContactInformation(ReplaceUserPayload payload, User user, User? reference, ActorId actorId)
  {
    Address? address = payload.Address?.ToAddress(_addressHelper);
    if (reference == null || address != reference.Address)
    {
      user.SetAddress(address, actorId);
    }

    Email? email = payload.Email?.ToEmail();
    if (reference == null || email != reference.Email)
    {
      user.SetEmail(email, actorId);
    }

    Phone? phone = payload.Phone?.ToPhone();
    if (reference == null || phone != reference.Phone)
    {
      user.SetPhone(phone, actorId);
    }
  }

  private static void ReplacePersonalInformation(ReplaceUserPayload payload, User user, User? reference)
  {
    PersonName? firstName = PersonName.TryCreate(payload.FirstName);
    if (reference == null || firstName != reference.FirstName)
    {
      user.FirstName = firstName;
    }
    PersonName? middleName = PersonName.TryCreate(payload.MiddleName);
    if (reference == null || middleName != reference.MiddleName)
    {
      user.MiddleName = middleName;
    }
    PersonName? lastName = PersonName.TryCreate(payload.LastName);
    if (reference == null || lastName != reference.LastName)
    {
      user.LastName = lastName;
    }
    PersonName? nickname = PersonName.TryCreate(payload.Nickname);
    if (reference == null || nickname != reference.Nickname)
    {
      user.Nickname = nickname;
    }

    if (reference == null || payload.Birthdate != reference.Birthdate)
    {
      user.Birthdate = payload.Birthdate;
    }
    Gender? gender = Gender.TryCreate(payload.Gender);
    if (reference == null || gender != reference.Gender)
    {
      user.Gender = gender;
    }
    Locale? locale = Locale.TryCreate(payload.Locale);
    if (reference == null || locale != reference.Locale)
    {
      user.Locale = locale;
    }
    TimeZone? timeZone = TimeZone.TryCreate(payload.TimeZone);
    if (reference == null || timeZone != reference.TimeZone)
    {
      user.TimeZone = timeZone;
    }

    Url? picture = Url.TryCreate(payload.Picture);
    if (reference == null || picture != reference.Picture)
    {
      user.Picture = picture;
    }
    Url? profile = Url.TryCreate(payload.Profile);
    if (reference == null || profile != reference.Profile)
    {
      user.Profile = profile;
    }
    Url? website = Url.TryCreate(payload.Website);
    if (reference == null || website != reference.Website)
    {
      user.Website = website;
    }
  }

  private static void ReplaceCustomAttributes(ReplaceUserPayload payload, User user, User? reference)
  {
    HashSet<Identifier> payloadKeys = new(capacity: payload.CustomAttributes.Count);

    IEnumerable<Identifier> referenceKeys;
    if (reference == null)
    {
      referenceKeys = user.CustomAttributes.Keys;

      foreach (CustomAttribute customAttribute in payload.CustomAttributes)
      {
        Identifier key = new(customAttribute.Key);
        payloadKeys.Add(key);
        user.SetCustomAttribute(key, customAttribute.Value);
      }
    }
    else
    {
      referenceKeys = reference.CustomAttributes.Keys;

      foreach (CustomAttribute customAttribute in payload.CustomAttributes)
      {
        Identifier key = new(customAttribute.Key);
        payloadKeys.Add(key);

        string value = customAttribute.Value.Trim();
        if (!reference.CustomAttributes.TryGetValue(key, out string? existingValue) || value != existingValue)
        {
          user.SetCustomAttribute(key, value);
        }
      }
    }

    foreach (Identifier key in referenceKeys)
    {
      if (!payloadKeys.Contains(key))
      {
        user.RemoveCustomAttribute(key);
      }
    }
  }

  private async Task ReplaceRolesAsync(ReplaceUserPayload payload, User user, User? reference, ActorId actorId, CancellationToken cancellationToken)
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
        //Role role = new(roleId.AggregateId);
        //user.RemoveRole(role, actorId); // TODO(fpion): implement
      }
    }
  }
}
