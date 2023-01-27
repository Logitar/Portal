using Logitar.Portal.Core;
using Logitar.Portal.Core.Users.Events;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class UserEntity : AggregateEntity
  {
    public UserEntity(UserCreatedEvent @event) : base(@event)
    {
      Username = @event.Username;
      PasswordHash = @event.PasswordHash;
      PasswordChangedOn = @event.PasswordHash == null ? null : @event.OccurredOn;

      Email = @event.Email;
      PhoneNumber = @event.PhoneNumber;

      FirstName = @event.FirstName;
      MiddleName = @event.MiddleName;
      LastName = @event.LastName;

      Locale = @event.Locale;
      Picture = @event.Picture;
    }
    private UserEntity() : base()
    {
    }

    public int UserId { get; private set; }

    public RealmEntity? Realm { get; private set; }
    public int? RealmId { get; private set; }

    public string Username { get; private set; } = null!;
    public string UsernameNormalized
    {
      get => Username.ToUpper();
      private set { }
    }
    public string? PasswordHash { get; private set; }
    public DateTime? PasswordChangedOn { get; private set; }
    public bool HasPassword
    {
      get => PasswordHash != null && PasswordChangedOn.HasValue;
      private set { }
    }

    public string? Email { get; private set; }
    public string? EmailNormalized
    {
      get => Email?.ToUpper();
      private set { }
    }
    public string? EmailConfirmedBy { get; private set; }
    public DateTime? EmailConfirmedOn { get; private set; }
    public bool IsEmailConfirmed
    {
      get => EmailConfirmedBy != null && EmailConfirmedOn.HasValue;
      private set { }
    }
    public string? PhoneNumber { get; private set; }
    public string? PhoneNumberNormalized
    {
      get => PhoneNumber;
      private set { }
    }
    public string? PhoneNumberConfirmedBy { get; private set; }
    public DateTime? PhoneNumberConfirmedOn { get; private set; }
    public bool IsPhoneNumberConfirmed
    {
      get => PhoneNumberConfirmedBy != null && PhoneNumberConfirmedOn.HasValue;
      private set { }
    }
    public bool IsAccountConfirmed
    {
      get => IsEmailConfirmed || IsPhoneNumberConfirmed;
      private set { }
    }

    public string? FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string? LastName { get; private set; }
    public string? FullName
    {
      get => string.Join(' ', new[] { FirstName, MiddleName, LastName }
        .Where(name => !string.IsNullOrWhiteSpace(name))
        .Select(name => name!.Trim())).CleanTrim();
      private set { }
    }

    public string? Locale { get; private set; }
    public string? Picture { get; private set; }

    public DateTime? SignedInOn { get; private set; }

    public string? DisabledBy { get; private set; }
    public DateTime? DisabledOn { get; private set; }
    public bool IsDisabled
    {
      get => DisabledBy != null && DisabledOn.HasValue;
      private set { }
    }

    public List<SessionEntity> Sessions { get; private set; } = new();

    public void ChangePassword(UserChangedPasswordEvent @event)
    {
      Update(@event);

      PasswordHash = @event.PasswordHash;
      PasswordChangedOn = @event.OccurredOn;
    }
    public void ConfirmEmail(UserConfirmedEmailEvent @event)
    {
      EmailConfirmedBy = @event.UserId.ToString();
      EmailConfirmedOn = @event.OccurredOn;
    }
    public void ConfirmPhoneNumber(UserConfirmedPhoneNumberEvent @event)
    {
      PhoneNumberConfirmedBy = @event.UserId.ToString();
      PhoneNumberConfirmedOn = @event.OccurredOn;
    }
    public void Disable(UserDisabledEvent @event)
    {
      DisabledBy = @event.UserId.ToString();
      DisabledOn = @event.OccurredOn;
    }
    public void Enable()
    {
      DisabledBy = null;
      DisabledOn = null;
    }
    public void SignIn(UserSignedInEvent @event)
    {
      SignedInOn = @event.OccurredOn;
    }
    public void Update(UserUpdatedEvent @event)
    {
      base.Update(@event);

      if (PasswordHash != null)
      {
        PasswordHash = @event.PasswordHash;
        PasswordChangedOn = @event.OccurredOn;
      }

      if (@event.HasEmailChanged)
      {
        Email = @event.Email;
        EmailConfirmedBy = null;
        EmailConfirmedOn = null;
      }
      if (@event.HasPhoneNumberChanged)
      {
        PhoneNumber = @event.PhoneNumber;
        PhoneNumberConfirmedBy = null;
        PhoneNumberConfirmedOn = null;
      }

      FirstName = @event.FirstName;
      MiddleName = @event.MiddleName;
      LastName = @event.LastName;

      Locale = @event.Locale;
      Picture = @event.Picture;
    }
  }
}
