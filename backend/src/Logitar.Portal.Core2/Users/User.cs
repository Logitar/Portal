using Logitar.Portal.Core2.Users.Events;
using System.Globalization;

namespace Logitar.Portal.Core2.Users
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

    public string Username { get; private set; } = null!;
    public string? PasswordHash { get; private set; }

    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }

    public string? FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string? LastName { get; private set; }
    public string? FullName => string.Join(' ', new[] { FirstName, MiddleName, LastName }
      .Where(name => !string.IsNullOrWhiteSpace(name))
      .Select(name => name!.Trim())).CleanTrim();

    public CultureInfo? Locale { get; private set; }
    public string? Picture { get; private set; }

    protected virtual void Apply(UserCreatedEvent @event)
    {
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

    public override string ToString() => $"{(FullName == null ? Username : $"{FullName} ({Username})")} | {base.ToString()}";
  }
}
