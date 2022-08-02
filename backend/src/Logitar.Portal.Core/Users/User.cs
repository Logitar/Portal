using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Users.Events;
using Logitar.Portal.Core.Users.Payloads;
using System.Globalization;

namespace Logitar.Portal.Core.Users
{
  public class User : Aggregate
  {
    public User(CreateUserSecurePayload payload, Guid userId, Realm? realm = null)
    {
      ApplyChange(new CreatedEvent(payload, userId));

      Realm = realm;
      RealmSid = realm?.Sid;
    }
    private User()
    {
    }

    public Realm? Realm { get; private set; }
    public int? RealmSid { get; private set; }

    public string Username { get; private set; } = null!;
    public string UsernameNormalized
    {
      get => Username.ToUpper();
      private set { /* EntityFrameworkCore only setter */ }
    }
    public string? PasswordHash { get; private set; }

    public string? Email { get; private set; }
    public string? EmailNormalized
    {
      get => Email?.ToUpper();
      private set { /* EntityFrameworkCore only setter */ }
    }
    public DateTime? EmailConfirmedAt { get; private set; }
    public Guid? EmailConfirmedById { get; private set; }
    public bool IsEmailConfirmed
    {
      get => EmailConfirmedAt.HasValue && EmailConfirmedById.HasValue;
      private set { /* EntityFrameworkCore only setter */ }
    }
    public string? PhoneNumber { get; private set; }
    public DateTime? PhoneNumberConfirmedAt { get; private set; }
    public Guid? PhoneNumberConfirmedById { get; private set; }
    public bool IsPhoneNumberConfirmed
    {
      get => PhoneNumberConfirmedAt.HasValue && PhoneNumberConfirmedById.HasValue;
      private set { /* EntityFrameworkCore only setter */ }
    }
    public bool IsAccountConfirmed
    {
      get => IsEmailConfirmed || IsPhoneNumberConfirmed;
      private set { /* EntityFrameworkCore only setter */ }
    }

    public string? FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string? LastName { get; private set; }
    public string? FullName => string.Join(' ', new[] { FirstName, MiddleName, LastName }
      .Where(name => name != null)).CleanTrim();

    public CultureInfo? Culture => Locale == null ? null : CultureInfo.GetCultureInfo(Locale);
    public string? Locale { get; private set; }
    public string? Picture { get; private set; }

    public DateTime? PasswordChangedAt { get; private set; }
    public DateTime? SignedInAt { get; private set; }

    public DateTime? DisabledAt { get; private set; }
    public Guid? DisabledById { get; private set; }
    public bool IsDisabled
    {
      get => DisabledAt.HasValue && DisabledById.HasValue;
      private set { /* EntityFrameworkCore only setter */ }
    }

    public List<Session> Sessions { get; private set; } = new();

    public void ConfirmEmail(Guid? userId = null) => ApplyChange(new ConfirmedEmailEvent(userId ?? Id));
    public void ConfirmPhoneNumber(Guid? userId = null) => ApplyChange(new ConfirmedPhoneNumberEvent(userId ?? Id));
    public void Delete(Guid userId) => ApplyChange(new DeletedEvent(userId));
    public void Update(UpdateUserSecurePayload payload, Guid userId) => ApplyChange(new UpdatedEvent(payload, userId));

    public void ChangePassword(string passwordHash) => ApplyChange(new ChangedPasswordEvent(passwordHash, Id));
    public void SignIn(DateTime signedInAt) => ApplyChange(new SignedInEvent(signedInAt, Id));

    public void Disable(Guid userId) => ApplyChange(new DisabledEvent(userId));
    public void Enable(Guid userId) => ApplyChange(new EnabledEvent(userId));

    protected virtual void Apply(ConfirmedEmailEvent @event)
    {
      EmailConfirmedAt = @event.OccurredAt;
      EmailConfirmedById = @event.UserId;
    }
    protected virtual void Apply(ConfirmedPhoneNumberEvent @event)
    {
      PhoneNumberConfirmedAt = @event.OccurredAt;
      PhoneNumberConfirmedById = @event.UserId;
    }
    protected virtual void Apply(CreatedEvent @event)
    {
      Username = @event.Payload.Username;

      PasswordChangedAt = @event.OccurredAt;
      PasswordHash = @event.Payload.PasswordHash;

      Apply(@event.Payload);
    }
    protected virtual void Apply(ChangedPasswordEvent @event)
    {
      PasswordChangedAt = @event.OccurredAt;
      PasswordHash = @event.PasswordHash;
    }
    protected virtual void Apply(DeletedEvent @event)
    {
    }
    protected virtual void Apply(DisabledEvent @event)
    {
      DisabledAt = @event.OccurredAt;
      DisabledById = @event.UserId;
    }
    protected virtual void Apply(EnabledEvent @event)
    {
      DisabledAt = null;
      DisabledById = null;
    }
    protected virtual void Apply(SignedInEvent @event)
    {
      SignedInAt = @event.OccurredAt;
    }
    protected virtual void Apply(UpdatedEvent @event)
    {
      if (@event.Payload.PasswordHash != null)
      {
        PasswordChangedAt = @event.OccurredAt;
        PasswordHash = @event.Payload.PasswordHash;
      }

      Apply(@event.Payload);
    }

    private void Apply(SaveUserPayload payload)
    {
      if (Email?.ToUpper() != payload.Email?.ToUpper())
      {
        Email = payload.Email;
        EmailConfirmedAt = null;
        EmailConfirmedById = null;
      }

      if (PhoneNumber?.ToUpper() != payload.PhoneNumber?.ToUpper())
      {
        PhoneNumber = payload.PhoneNumber;
        PhoneNumberConfirmedAt = null;
        PhoneNumberConfirmedById = null;
      }

      FirstName = payload.FirstName?.CleanTrim();
      LastName = payload.LastName?.CleanTrim();
      MiddleName = payload.MiddleName?.CleanTrim();

      Locale = payload.Locale;
      Picture = payload.Picture;
    }

    public override string ToString() => $"{Username} | {base.ToString()}";
  }
}
