using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users.Events;
using System.Globalization;

namespace Logitar.Portal.Domain.Users
{
  public class User : AggregateRoot
  {
    public User(AggregateId userId, string username, Realm? realm = null, string? email = null, bool isEmailConfirmed = false,
      string? firstName = null, string? lastName = null, CultureInfo? locale = null, string? picture = null)
      : this(userId, username, realm, passwordHash: null,
          email, isEmailConfirmed, phoneNumber: null, isPhoneNumberConfirmed: false,
          firstName, middleName: null, lastName,
          locale, picture)
    {
    }
    public User(AggregateId userId, string username, Realm? realm = null, string? passwordHash = null,
      string? email = null, bool isEmailConfirmed = false, string? phoneNumber = null, bool isPhoneNumberConfirmed = false,
      string? firstName = null, string? middleName = null, string? lastName = null,
      CultureInfo? locale = null, string? picture = null) : base()
    {
      ApplyChange(new UserCreatedEvent
      {
        RealmId = realm?.Id,
        Username = username.Trim(),
        PasswordHash = passwordHash,
        Email = email?.CleanTrim(),
        IsEmailConfirmed = isEmailConfirmed,
        PhoneNumber = phoneNumber?.CleanTrim(),
        IsPhoneNumberConfirmed = isPhoneNumberConfirmed,
        FirstName = firstName?.CleanTrim(),
        MiddleName = middleName?.CleanTrim(),
        LastName = lastName?.CleanTrim(),
        FullName = GetFullName(firstName, middleName, lastName),
        Locale = locale,
        Picture = picture?.CleanTrim()
      }, userId);
    }
    public User(string username, string passwordHash, string email, string firstName, string lastName, CultureInfo locale) : base()
    {
      ApplyChange(new UserCreatedEvent
      {
        Username = username.Trim(),
        PasswordHash = passwordHash,
        Email = email.Trim(),
        FirstName = firstName.Trim(),
        LastName = lastName.Trim(),
        FullName = GetFullName(firstName, lastName),
        Locale = locale
      }, Id);
    }
    private User() : base()
    {
    }

    public AggregateId? RealmId { get; private set; }

    public string Username { get; private set; } = string.Empty;
    public string? PasswordHash { get; private set; }
    public bool HasPassword => PasswordHash != null;

    public string? Email { get; private set; }
    public bool IsEmailConfirmed { get; private set; }
    public string? PhoneNumber { get; private set; }
    public bool IsPhoneNumberConfirmed { get; private set; }
    public bool IsAccountConfirmed => IsEmailConfirmed || IsPhoneNumberConfirmed;

    public string? FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string? LastName { get; private set; }
    public string? FullName { get; private set; }

    public CultureInfo? Locale { get; private set; }
    public string? Picture { get; private set; }

    public bool IsDisabled { get; private set; }

    public List<ExternalProvider> ExternalProviders { get; private set; } = new();

    public void AddExternalProvider(AggregateId userId, string key, string value, string? displayName = null)
    {
      ApplyChange(new UserAddedExternalProviderEvent
      {
        Key = key,
        Value = value,
        DisplayName = displayName
      }, userId);
    }
    public void ChangePassword(string passwordHash) => ApplyChange(new UserChangedPasswordEvent
    {
      PasswordHash = passwordHash
    }, Id);
    public void Delete(AggregateId userId)
    {
      if (userId == Id)
      {
        throw new UserCannotDeleteItselfException(this);
      }

      ApplyChange(new UserDeletedEvent(), userId);
    }
    public void Disable(AggregateId userId)
    {
      if (IsDisabled)
      {
        throw new UserAlreadyDisabledException(this);
      }
      else if (userId == Id)
      {
        throw new UserCannotDisableItselfException(this);
      }

      ApplyChange(new UserDisabledEvent(), userId);
    }
    public void Enable(AggregateId userId)
    {
      if (!IsDisabled)
      {
        throw new UserNotDisabledException(this);
      }

      ApplyChange(new UserEnabledEvent(), userId);
    }
    public void SignIn() => ApplyChange(new UserSignedInEvent(), Id);
    public void Update(AggregateId userId, string? passwordHash = null,
      string? email = null, string? phoneNumber = null,
      string? firstName = null, string? middleName = null, string? lastName = null,
      CultureInfo? locale = null, string? picture = null)
    {
      email = email?.CleanTrim();
      phoneNumber = phoneNumber?.CleanTrim();

      ApplyChange(new UserUpdatedEvent
      {
        PasswordHash = passwordHash,
        HasEmailChanged = email?.ToUpper() != Email?.ToUpper(),
        Email = email,
        HasPhoneNumberChanged = phoneNumber?.ToUpper() != PhoneNumber?.ToUpper(),
        PhoneNumber = phoneNumber,
        FirstName = firstName?.CleanTrim(),
        MiddleName = middleName?.CleanTrim(),
        LastName = lastName?.CleanTrim(),
        FullName = GetFullName(firstName, middleName, lastName),
        Locale = locale,
        Picture = picture?.CleanTrim()
      }, userId);
    }

    protected virtual void Apply(UserAddedExternalProviderEvent @event)
    {
      ExternalProviders.Add(new ExternalProvider(@event.Key, @event.Value, @event.DisplayName));
    }
    protected virtual void Apply(UserChangedPasswordEvent @event)
    {
      PasswordHash = @event.PasswordHash;
    }
    protected virtual void Apply(UserCreatedEvent @event)
    {
      RealmId = @event.RealmId;

      Username = @event.Username;
      PasswordHash = @event.PasswordHash;

      Email = @event.Email;
      IsEmailConfirmed = @event.IsEmailConfirmed;
      PhoneNumber = @event.PhoneNumber;
      IsPhoneNumberConfirmed = @event.IsPhoneNumberConfirmed;

      FirstName = @event.FirstName;
      MiddleName = @event.MiddleName;
      LastName = @event.LastName;
      FullName = @event.FullName;

      Locale = @event.Locale;
      Picture = @event.Picture;
    }
    protected virtual void Apply(UserDeletedEvent @event)
    {
      Delete();
    }
    protected virtual void Apply(UserDisabledEvent @event)
    {
      IsDisabled = true;
    }
    protected virtual void Apply(UserEnabledEvent @event)
    {
      IsDisabled = false;
    }
    protected virtual void Apply(UserSignedInEvent @event)
    {
    }
    protected virtual void Apply(UserUpdatedEvent @event)
    {
      PasswordHash = @event.PasswordHash ?? PasswordHash;

      if (@event.HasEmailChanged)
      {
        Email = @event.Email;
        IsEmailConfirmed = false;
      }
      if (@event.HasPhoneNumberChanged)
      {
        PhoneNumber = @event.PhoneNumber;
        IsPhoneNumberConfirmed = false;
      }

      FirstName = @event.FirstName;
      MiddleName = @event.MiddleName;
      LastName = @event.LastName;
      FullName = @event.FullName;

      Locale = @event.Locale;
      Picture = @event.Picture;
    }

    public override string ToString() => $"{(FullName == null ? Username : $"{FullName} ({Username})")} | {base.ToString()}";

    private static string? GetFullName(params string?[] names) => string.Join(' ', names
      .Where(name => !string.IsNullOrWhiteSpace(name))
      .Select(name => name!.Trim())).CleanTrim();
  }
}
