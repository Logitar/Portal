﻿using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Application.Users.Commands;

internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordService _passwordService;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;
  private readonly IOptions<UserSettings> _userSettings;

  public UpdateUserCommandHandler(IApplicationContext applicationContext,
    IPasswordService passwordService, IUserManager userManager, IUserQuerier userQuerier,
    IUserRepository userRepository, IOptions<UserSettings> userSettings)
  {
    _applicationContext = applicationContext;
    _passwordService = passwordService;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
    _userSettings = userSettings;
  }

  public async Task<User?> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = command.Id.GetAggregateId(nameof(command.Id));
    UserAggregate? user = await _userRepository.LoadAsync(id, cancellationToken);
    if (user == null)
    {
      return null;
    }

    UpdateUserPayload payload = command.Payload;

    if (payload.UniqueName != null)
    {
      UserSettings userSettings = _userSettings.Value;
      user.SetUniqueName(userSettings.UniqueNameSettings, payload.UniqueName);
    }
    if (payload.Password != null)
    {
      Password newPassword = _passwordService.Create(payload.Password.NewPassword);
      if (payload.Password.CurrentPassword == null)
      {
        user.SetPassword(newPassword);
      }
      else
      {
        user.ChangePassword(payload.Password.CurrentPassword, newPassword);
      }
    }
    if (payload.IsDisabled.HasValue)
    {
      if (payload.IsDisabled.Value)
      {
        user.Disable();
      }
      else
      {
        user.Enable();
      }
    }

    if (payload.Address != null)
    {
      bool isVerified = payload.Address.Value?.IsVerified ?? user.Address?.IsVerified ?? false;
      user.Address = payload.Address.Value?.ToPostalAddress(isVerified);
    }
    if (payload.Email != null)
    {
      bool isVerified = payload.Email.Value?.IsVerified ?? user.Email?.IsVerified ?? false;
      user.Email = payload.Email.Value?.ToEmailAddress(isVerified);
    }
    if (payload.Phone != null)
    {
      bool isVerified = payload.Phone.Value?.IsVerified ?? user.Phone?.IsVerified ?? false;
      user.Phone = payload.Phone.Value?.ToPhoneNumber(isVerified);
    }

    if (payload.FirstName != null)
    {
      user.FirstName = payload.FirstName.Value;
    }
    if (payload.MiddleName != null)
    {
      user.MiddleName = payload.MiddleName.Value;
    }
    if (payload.LastName != null)
    {
      user.LastName = payload.LastName.Value;
    }
    if (payload.Nickname != null)
    {
      user.Nickname = payload.Nickname.Value;
    }

    if (payload.Birthdate != null)
    {
      user.Birthdate = payload.Birthdate.Value;
    }
    if (payload.Gender != null)
    {
      user.Gender = payload.Gender.Value?.GetGender(nameof(payload.Gender));
    }
    if (payload.Locale != null)
    {
      user.Locale = payload.Locale.Value?.GetLocale(nameof(payload.Locale));
    }
    if (payload.TimeZone != null)
    {
      user.TimeZone = payload.TimeZone.Value?.GetTimeZone(nameof(payload.TimeZone));
    }

    if (payload.Picture != null)
    {
      user.Picture = payload.Picture.Value?.GetUrl(nameof(payload.Picture));
    }
    if (payload.Profile != null)
    {
      user.Profile = payload.Profile.Value?.GetUrl(nameof(payload.Profile));
    }
    if (payload.Website != null)
    {
      user.Website = payload.Website.Value?.GetUrl(nameof(payload.Website));
    }

    foreach (CustomAttributeModification customAttribute in payload.CustomAttributes)
    {
      if (customAttribute.Value == null)
      {
        user.RemoveCustomAttribute(customAttribute.Key);
      }
      else
      {
        user.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
      }
    }

    if (user.HasChanges)
    {
      user.Update(_applicationContext.ActorId);

      await _userManager.SaveAsync(user, cancellationToken);
    }

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
