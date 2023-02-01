using Logitar.Portal.Domain.Users.Events;
using System.Globalization;

namespace Logitar.Portal.Domain.Users
{
  public class User : AggregateRoot
  {
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
        LocaleName = locale.Name
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

    public void ChangePassword(string passwordHash) => ApplyChange(new UserChangedPasswordEvent
    {
      PasswordHash = passwordHash
    }, Id);
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
        LocaleName = locale?.Name,
        Picture = picture?.CleanTrim()
      }, userId);
    }

    protected virtual void Apply(UserChangedPasswordEvent @event)
    {
      PasswordHash = @event.PasswordHash;
    }
    protected virtual void Apply(UserCreatedEvent @event)
    {
      Username = @event.Username;
      PasswordHash = @event.PasswordHash;

      Email = @event.Email;

      FirstName = @event.FirstName;
      LastName = @event.LastName;
      FullName = @event.FullName;

      Locale = @event.LocaleName == null ? null : CultureInfo.GetCultureInfo(@event.LocaleName);
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

      Locale = @event.LocaleName == null ? null : CultureInfo.GetCultureInfo(@event.LocaleName);
      Picture = @event.Picture;
    }

    public override string ToString() => $"{(FullName == null ? Username : $"{FullName} ({Username})")} | {base.ToString()}";

    private static string? GetFullName(params string?[] names) => string.Join(' ', names
      .Where(name => !string.IsNullOrWhiteSpace(name))
      .Select(name => name!.Trim())).CleanTrim();
  }
}
