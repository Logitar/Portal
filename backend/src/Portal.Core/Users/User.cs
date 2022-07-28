using Portal.Core.Realms;
using Portal.Core.Sessions;
using Portal.Core.Users.Events;
using Portal.Core.Users.Payloads;
using System.Globalization;

namespace Portal.Core.Users
{
  public class User : Aggregate
  {
    public User(CreateUserPayload payload, Guid userId, string? passwordHash = null, Realm? realm = null)
    {
      ApplyChange(new CreatedEvent(payload, userId, passwordHash));

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
    public DateTime? EmailConfirmed { get; private set; }
    public string? PhoneNumber { get; private set; }
    public DateTime? PhoneNumberConfirmed { get; private set; }

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

    public List<Session> Sessions { get; private set; } = new();

    public void Delete(Guid userId) => ApplyChange(new DeletedEvent(userId));
    public void Update(UpdateUserPayload payload, Guid userId) => ApplyChange(new UpdatedEvent(payload, userId));

    public void ChangePassword(string passwordHash) => ApplyChange(new ChangedPasswordEvent(passwordHash, Id));
    public void SignIn(DateTime signedInAt) => ApplyChange(new SignedInEvent(signedInAt, Id));

    protected virtual void Apply(CreatedEvent @event)
    {
      Username = @event.Payload.Username;

      if (@event.PasswordHash != null)
      {
        PasswordChangedAt = @event.OccurredAt;
        PasswordHash = @event.PasswordHash;
      }

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
    protected virtual void Apply(SignedInEvent @event)
    {
      SignedInAt = @event.OccurredAt;
    }
    protected virtual void Apply(UpdatedEvent @event)
    {
      Apply(@event.Payload);
    }

    private void Apply(SaveUserPayload payload)
    {
      Email = payload.Email;
      PhoneNumber = payload.PhoneNumber?.CleanTrim();

      FirstName = payload.FirstName?.CleanTrim();
      LastName = payload.LastName?.CleanTrim();
      MiddleName = payload.MiddleName?.CleanTrim();

      Locale = payload.Locale;
      Picture = payload.Picture;
    }

    public override string ToString() => $"{Username} | {base.ToString()}";
  }
}
