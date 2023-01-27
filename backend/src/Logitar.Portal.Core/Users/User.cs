using Logitar.Portal.Core.Accounts;
using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Core.Users.Events;
using System.Globalization;

namespace Logitar.Portal.Core.Users
{
  public class User : AggregateRoot
  {
    public User(AggregateId id, string username, string passwordHash, string email,
      string firstName, string lastName, CultureInfo locale) : base(id)
    {
      ApplyChange(new UserCreatedEvent
      {
        Username = username.Trim(),
        PasswordHash = passwordHash,
        Email = email.Trim(),
        FirstName = firstName.Trim(),
        LastName = lastName.Trim(),
        Locale = locale.Name,
      }, Id);
    }
    public User(string? realmId, string username, string? passwordHash, string? email, string? phoneNumber,
      string? firstName, string? middleName, string? lastName,
      CultureInfo? locale, string? picture, AggregateId userId)
    {
      ApplyChange(new UserCreatedEvent
      {
        RealmId = realmId,
        Username = username.Trim(),
        PasswordHash = passwordHash,
        Email = email?.CleanTrim(),
        PhoneNumber = phoneNumber?.CleanTrim(),
        FirstName = firstName?.CleanTrim(),
        MiddleName = middleName?.CleanTrim(),
        LastName = lastName?.CleanTrim(),
        Locale = locale?.Name,
        Picture = picture?.CleanTrim()
      }, userId);
    }

    public AggregateId? RealmId { get; private set; }

    public string Username { get; private set; } = null!;
    public string? PasswordHash { get; private set; }

    public string? Email { get; private set; }
    public bool IsEmailConfirmed { get; private set; }
    public string? PhoneNumber { get; private set; }
    public bool IsPhoneNumberConfirmed { get; private set; }
    public bool IsAccountConfirmed => IsEmailConfirmed || IsPhoneNumberConfirmed;

    public string? FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string? LastName { get; private set; }
    public string? FullName => string.Join(' ', new[] { FirstName, MiddleName, LastName }
      .Where(name => !string.IsNullOrWhiteSpace(name))
      .Select(name => name!.Trim())).CleanTrim();

    public CultureInfo? Locale { get; private set; }
    public string? Picture { get; private set; }

    public bool IsDisabled { get; private set; }

    public void EnsureIsTrusted(RealmModel? realm)
    {
      if (realm?.RequireUniqueEmail == true && !IsAccountConfirmed)
      {
        throw new AccountNotConfirmedException(Id);
      }
      else if (IsDisabled)
      {
        throw new AccountIsDisabledException(Id);
      }
    }

    public void ChangePassword(string passwordHash)
    {
      if (PasswordHash == null)
      {
        throw new UserHasNoPasswordException(this);
      }

      ApplyChange(new UserChangedPasswordEvent
      {
        PasswordHash = passwordHash
      }, Id);
    }
    public void ConfirmEmail(AggregateId userId) => ApplyChange(new UserConfirmedEmailEvent(), userId);
    public void ConfirmPhoneNumber(AggregateId userId) => ApplyChange(new UserConfirmedPhoneNumberEvent(), userId);
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
    public void Update(string? passwordHash, string? email, string? phoneNumber,
      string? firstName, string? middleName, string? lastName,
      CultureInfo? locale, string? picture, AggregateId userId)
    {
      ApplyChange(new UserUpdatedEvent
      {
        PasswordHash = passwordHash,
        Email = email,
        PhoneNumber = phoneNumber,
        FirstName = firstName,
        MiddleName = middleName,
        LastName = lastName,
        Locale = locale?.Name,
        Picture = picture
      }, userId);
    }

    protected virtual void Apply(UserChangedPasswordEvent @event)
    {
      PasswordHash = @event.PasswordHash;
    }
    protected virtual void Apply(UserConfirmedEmailEvent @event)
    {
      IsEmailConfirmed = true;
    }
    protected virtual void Apply(UserConfirmedPhoneNumberEvent @event)
    {
      IsPhoneNumberConfirmed = true;
    }
    protected virtual void Apply(UserCreatedEvent @event)
    {
      RealmId = @event.RealmId == null ? null : new AggregateId(@event.RealmId);

      Username = @event.Username;
      PasswordHash = @event.PasswordHash;

      Email = @event.Email;
      PhoneNumber = @event.PhoneNumber;

      FirstName = @event.FirstName;
      MiddleName = @event.MiddleName;
      LastName = @event.LastName;

      Locale = @event.Locale == null ? null : CultureInfo.GetCultureInfo(@event.Locale);
      Picture = @event.Picture;
    }
    protected virtual void Apply(UserDeletedEvent @event)
    {
      IsDeleted = true;
    }
    protected virtual void Apply(UserDisabledEvent @event)
    {
      IsDisabled = true;
    }
    protected virtual void Apply(UserSignedInEvent @event)
    {
    }
    protected virtual void Apply(UserUpdatedEvent @event)
    {
      if (@event.PasswordHash != null)
      {
        PasswordHash = @event.PasswordHash;
      }

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

      Locale = @event.Locale == null ? null : CultureInfo.GetCultureInfo(@event.Locale);
      Picture = @event.Picture;
    }

    public override string ToString() => $"{(FullName == null ? Username : $"{FullName} ({Username})")} | {base.ToString()}";
  }
}
