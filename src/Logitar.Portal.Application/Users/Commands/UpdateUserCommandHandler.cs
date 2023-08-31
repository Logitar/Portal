using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordService _passwordService;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public UpdateUserCommandHandler(IApplicationContext applicationContext, IPasswordService passwordService,
    IRealmRepository realmRepository, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _passwordService = passwordService;
    _realmRepository = realmRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
  {
    UserAggregate? user = await _userRepository.LoadAsync(command.Id, cancellationToken);
    if (user == null)
    {
      return null;
    }

    RealmAggregate? realm = null;
    if (user.TenantId != null)
    {
      realm = await _realmRepository.LoadAsync(user, cancellationToken);
    }
    string? tenantId = realm?.Id.Value;

    UpdateUserPayload payload = command.Payload;

    if (payload.UniqueName != null)
    {
      UserAggregate? other = await _userRepository.LoadAsync(tenantId, payload.UniqueName, cancellationToken);
      if (other?.Equals(user) == false)
      {
        throw new UniqueNameAlreadyUsedException<UserAggregate>(tenantId, payload.UniqueName, nameof(payload.UniqueName));
      }

      IUniqueNameSettings uniqueNameSettings = realm?.UniqueNameSettings ?? _applicationContext.Configuration.UniqueNameSettings;
      user.SetUniqueName(uniqueNameSettings, payload.UniqueName);
    }
    if (payload.Password != null)
    {
      IPasswordSettings passwordSettings = realm?.PasswordSettings ?? _applicationContext.Configuration.PasswordSettings;
      Password newPassword = _passwordService.Create(passwordSettings, payload.Password.NewPassword);
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
        user.Disable(_applicationContext.ActorId);
      }
      else
      {
        user.Enable(_applicationContext.ActorId);
      }
    }

    if (payload.Address != null)
    {
      user.Address = payload.Address.Value?.ToPostalAddress();
    }
    if (payload.Email != null)
    {
      EmailAddress? email = payload.Email.Value?.ToEmailAddress();
      if (email != null && realm?.RequireUniqueEmail == true)
      {
        IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(tenantId, email, cancellationToken);
        if (users.Any(u => !u.Equals(user)))
        {
          throw new EmailAddressAlreadyUsedException(tenantId, email, nameof(payload.Email));
        }
      }

      user.Email = email;
    }
    if (payload.Phone != null)
    {
      user.Phone = payload.Phone.Value?.ToPhoneNumber();
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
      user.Profile = payload.Profile.Value?.GetUrl(nameof(payload.Picture));
    }
    if (payload.Website != null)
    {
      user.Website = payload.Website.Value?.GetUrl(nameof(payload.Picture));
    }

    // TODO(fpion): Roles

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
    user.Update(_applicationContext.ActorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
