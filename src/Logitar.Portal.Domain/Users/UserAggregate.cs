using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Roles;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.Domain.Users.Events;
using Logitar.Portal.Domain.Users.Validators;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Users;

public class UserAggregate : AggregateRoot
{
  private readonly Dictionary<string, string> _customAttributes = new();
  private readonly HashSet<AggregateId> _roles = new();

  private Password? _password = null;

  private PostalAddress? _address = null;
  private EmailAddress? _email = null;
  private PhoneNumber? _phone = null;

  private string? _firstName = null;
  private string? _middleName = null;
  private string? _lastName = null;
  private string? _nickname = null;

  private DateTime? _birthdate = null;
  private Gender? _gender = null;
  private Locale? _locale = null;
  private TimeZoneEntry? _timeZone = null;

  private Uri? _picture = null;
  private Uri? _profile = null;
  private Uri? _website = null;

  public UserAggregate(AggregateId id) : base(id)
  {
  }

  public UserAggregate(IUniqueNameSettings uniqueNameSettings, string uniqueName, string? tenantId = null, ActorId actorId = default, AggregateId? id = null)
    : base(id)
  {
    uniqueName = uniqueName.Trim();
    new UniqueNameValidator(uniqueNameSettings, nameof(UniqueName)).ValidateAndThrow(uniqueName);

    tenantId = tenantId?.CleanTrim();
    if (tenantId != null)
    {
      new TenantIdValidator(nameof(TenantId)).ValidateAndThrow(tenantId);
    }

    ApplyChange(new UserCreatedEvent(actorId)
    {
      TenantId = tenantId,
      UniqueName = uniqueName
    });
  }
  protected virtual void Apply(UserCreatedEvent created)
  {
    TenantId = created.TenantId;

    UniqueName = created.UniqueName;
  }

  public string? TenantId { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public bool HasPassword => _password != null;
  public bool IsDisabled { get; private set; }
  public DateTime? AuthenticatedOn { get; private set; }

  public PostalAddress? Address
  {
    get => _address;
    set
    {
      if (value != _address)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.Address = new Modification<PostalAddress>(value);
        _address = value;
      }
    }
  }
  public EmailAddress? Email
  {
    get => _email;
    set
    {
      if (value != _email)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.Email = new Modification<EmailAddress>(value);
        _email = value;
      }
    }
  }
  public PhoneNumber? Phone
  {
    get => _phone;
    set
    {
      if (value != _phone)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.Phone = new Modification<PhoneNumber>(value);
        _phone = value;
      }
    }
  }
  public bool IsConfirmed => Address?.IsVerified == true || Email?.IsVerified == true || Phone?.IsVerified == true;

  public string? FirstName
  {
    get => _firstName;
    set
    {
      value = value?.CleanTrim();
      if (value != null)
      {
        new PersonNameValidator(nameof(FirstName)).ValidateAndThrow(value);
      }

      if (value != _firstName)
      {
        string? fullName = PersonHelper.BuildFullName(value, MiddleName, LastName);

        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.FirstName = new Modification<string>(value);
        updated.FullName = new Modification<string>(fullName);

        _firstName = value;
        FullName = fullName;
      }
    }
  }
  public string? MiddleName
  {
    get => _middleName;
    set
    {
      value = value?.CleanTrim();
      if (value != null)
      {
        new PersonNameValidator(nameof(MiddleName)).ValidateAndThrow(value);
      }

      if (value != _middleName)
      {
        string? fullName = PersonHelper.BuildFullName(FirstName, value, LastName);

        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.MiddleName = new Modification<string>(value);
        updated.FullName = new Modification<string>(fullName);

        _middleName = value;
        FullName = fullName;
      }
    }
  }
  public string? LastName
  {
    get => _lastName;
    set
    {
      value = value?.CleanTrim();
      if (value != null)
      {
        new PersonNameValidator(nameof(LastName)).ValidateAndThrow(value);
      }

      if (value != _lastName)
      {
        string? fullName = PersonHelper.BuildFullName(FirstName, MiddleName, value);

        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.LastName = new Modification<string>(value);
        updated.FullName = new Modification<string>(fullName);

        _lastName = value;
        FullName = fullName;
      }
    }
  }
  public string? FullName { get; private set; }
  public string? Nickname
  {
    get => _nickname;
    set
    {
      value = value?.CleanTrim();
      if (value != null)
      {
        new PersonNameValidator(nameof(Nickname)).ValidateAndThrow(value);
      }

      if (value != _nickname)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.Nickname = new Modification<string>(value);
        _nickname = value;
      }
    }
  }

  public DateTime? Birthdate
  {
    get => _birthdate;
    set
    {
      if (value.HasValue)
      {
        new BirthdateValidator(nameof(Birthdate)).ValidateAndThrow(value.Value);
      }

      if (value != _birthdate)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.Birthdate = new Modification<DateTime?>(value);
        _birthdate = value;
      }
    }
  }
  public Gender? Gender
  {
    get => _gender;
    set
    {
      if (value != _gender)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.Gender = new Modification<Gender>(value);
        _gender = value;
      }
    }
  }
  public Locale? Locale
  {
    get => _locale;
    set
    {
      if (value != _locale)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.Locale = new Modification<Locale>(value);
        _locale = value;
      }
    }
  }
  public TimeZoneEntry? TimeZone
  {
    get => _timeZone;
    set
    {
      if (value != _timeZone)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.TimeZone = new Modification<TimeZoneEntry>(value);
        _timeZone = value;
      }
    }
  }

  public Uri? Picture
  {
    get => _picture;
    set
    {
      if (value != _picture)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.Picture = new Modification<Uri>(value);
        _picture = value;
      }
    }
  }
  public Uri? Profile
  {
    get => _profile;
    set
    {
      if (value != _profile)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.Profile = new Modification<Uri>(value);
        _profile = value;
      }
    }
  }
  public Uri? Website
  {
    get => _website;
    set
    {
      if (value != _website)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.Website = new Modification<Uri>(value);
        _website = value;
      }
    }
  }

  public IReadOnlyDictionary<string, string> CustomAttributes => _customAttributes.AsReadOnly();

  public IImmutableSet<AggregateId> Roles => ImmutableHashSet.Create(_roles.ToArray());

  public void AddRole(RoleAggregate role)
  {
    AggregateId roleId = role.Id;
    if (!_roles.Contains(roleId))
    {
      UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
      updated.Roles[roleId.Value] = CollectionAction.Add;
      _roles.Add(roleId);
    }
  }

  public void ChangePassword(string currentPassword, Password newPassword, ActorId? actorId = null)
  {
    if (_password?.IsMatch(currentPassword) != true)
    {
      throw new IncorrectUserPasswordException(this, currentPassword);
    }

    actorId ??= new(Id.Value);

    ApplyChange(new UserChangedPasswordEvent(actorId.Value, newPassword));
  }
  protected virtual void Apply(UserChangedPasswordEvent changedPassword) => _password = changedPassword.NewPassword;

  public void Delete(ActorId actorId = default) => ApplyChange(new UserDeletedEvent(actorId));

  public void Disable(ActorId actorId = default)
  {
    if (!IsDisabled)
    {
      ApplyChange(new UserDisabledEvent(actorId));
    }
  }
  protected virtual void Apply(UserDisabledEvent _) => IsDisabled = true;

  public void Enable(ActorId actorId = default)
  {
    if (IsDisabled)
    {
      ApplyChange(new UserEnabledEvent(actorId));
    }
  }
  protected virtual void Apply(UserEnabledEvent _) => IsDisabled = false;

  public void RemoveCustomAttribute(string key)
  {
    key = key.Trim();
    if (_customAttributes.ContainsKey(key))
    {
      UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
      updated.CustomAttributes[key] = null;
      _customAttributes.Remove(key);
    }
  }

  public void RemoveRole(RoleAggregate role) => RemoveRole(role.Id);
  public void RemoveRole(AggregateId roleId)
  {
    if (_roles.Contains(roleId))
    {
      UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
      updated.Roles[roleId.Value] = CollectionAction.Remove;
      _roles.Remove(roleId);
    }
  }

  public void SetCustomAttribute(string key, string value)
  {
    key = key.Trim();
    value = value.Trim();
    new CustomAttributeValidator().ValidateAndThrow(key, value);

    if (!_customAttributes.TryGetValue(key, out string? existingValue) || value != existingValue)
    {
      UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
      updated.CustomAttributes[key] = value;
      _customAttributes[key] = value;
    }
  }

  public void SetPassword(Password password)
  {
    UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
    updated.Password = password;
    _password = password;
  }

  public void SetUniqueName(IUniqueNameSettings uniqueNameSettings, string uniqueName)
  {
    uniqueName = uniqueName.Trim();
    new UniqueNameValidator(uniqueNameSettings, nameof(UniqueName)).ValidateAndThrow(uniqueName);

    if (uniqueName != UniqueName)
    {
      UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
      updated.UniqueName = uniqueName;
      UniqueName = uniqueName;
    }
  }

  public SessionAggregate SignIn(IUserSettings userSettings, Password? secret = null, ActorId? actorId = null)
  {
    return SignIn(userSettings, password: null, secret, actorId);
  }
  public SessionAggregate SignIn(IUserSettings userSettings, string? password, Password? secret = null, ActorId? actorId = null)
  {
    if (password != null && _password?.IsMatch(password) != true)
    {
      throw new IncorrectUserPasswordException(this, password);
    }

    if (IsDisabled)
    {
      throw new UserIsDisabledException(this);
    }
    else if (userSettings.RequireConfirmedAccount && !IsConfirmed)
    {
      throw new UserIsNotConfirmedException(this);
    }

    actorId ??= new(Id.Value);

    SessionAggregate session = new(this, secret, actorId.Value);

    ApplyChange(new UserSignedInEvent(actorId.Value, session.CreatedOn));

    return session;
  }
  protected virtual void Apply(UserSignedInEvent signedIn) => AuthenticatedOn = signedIn.OccurredOn;

  public void Update(ActorId actorId)
  {
    foreach (DomainEvent change in Changes)
    {
      if (change is UserUpdatedEvent updated && updated.ActorId == default)
      {
        updated.ActorId = actorId;

        if (updated.Version == Version)
        {
          UpdatedBy = actorId;
        }
      }
    }
  }

  protected virtual void Apply(UserUpdatedEvent updated)
  {
    if (updated.UniqueName != null)
    {
      UniqueName = updated.UniqueName;
    }
    if (updated.Password != null)
    {
      _password = updated.Password;
    }

    if (updated.Address != null)
    {
      _address = updated.Address.Value;
    }
    if (updated.Email != null)
    {
      _email = updated.Email.Value;
    }
    if (updated.Phone != null)
    {
      _phone = updated.Phone.Value;
    }

    if (updated.FirstName != null)
    {
      _firstName = updated.FirstName.Value;
    }
    if (updated.MiddleName != null)
    {
      _middleName = updated.MiddleName.Value;
    }
    if (updated.LastName != null)
    {
      _lastName = updated.LastName.Value;
    }
    if (updated.FullName != null)
    {
      FullName = updated.FullName.Value;
    }
    if (updated.Nickname != null)
    {
      Nickname = updated.Nickname.Value;
    }

    if (updated.Birthdate != null)
    {
      Birthdate = updated.Birthdate.Value;
    }
    if (updated.Gender != null)
    {
      _gender = updated.Gender.Value;
    }
    if (updated.Locale != null)
    {
      _locale = updated.Locale.Value;
    }
    if (updated.TimeZone != null)
    {
      _timeZone = updated.TimeZone.Value;
    }

    if (updated.Picture != null)
    {
      _picture = updated.Picture.Value;
    }
    if (updated.Profile != null)
    {
      _profile = updated.Profile.Value;
    }
    if (updated.Website != null)
    {
      _website = updated.Website.Value;
    }

    foreach (KeyValuePair<string, string?> customAttribute in updated.CustomAttributes)
    {
      if (customAttribute.Value == null)
      {
        _customAttributes.Remove(customAttribute.Key);
      }
      else
      {
        _customAttributes[customAttribute.Key] = customAttribute.Value;
      }
    }

    foreach (KeyValuePair<string, CollectionAction> role in updated.Roles)
    {
      AggregateId roleId = new(role.Key);

      switch (role.Value)
      {
        case CollectionAction.Add:
          _roles.Add(roleId);
          break;
        case CollectionAction.Remove:
          _roles.Remove(roleId);
          break;
      }
    }
  }

  protected virtual T GetLatestEvent<T>() where T : DomainEvent, new()
  {
    T? updated = Changes.SingleOrDefault(change => change is T) as T;
    if (updated == null)
    {
      updated = new();
      ApplyChange(updated);
    }

    return updated;
  }

  public override string ToString() => $"{FullName ?? UniqueName} | {base.ToString()}";
}
