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

    public string Username { get; private set; } = string.Empty;
    public string? PasswordHash { get; private set; }
    public bool HasPassword => PasswordHash != null;

    public string? Email { get; private set; }
    public bool IsAccountConfirmed { get; private set; }

    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? FullName { get; private set; }

    public CultureInfo? Locale { get; private set; }

    public bool IsDisabled { get; private set; }

    public void SignIn() => ApplyChange(new UserSignedInEvent(), Id);

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

    public override string ToString() => $"{(FullName == null ? Username : $"{FullName} ({Username})")} | {base.ToString()}";

    private static string? GetFullName(params string?[] names) => string.Join(' ', names
      .Where(name => !string.IsNullOrWhiteSpace(name))
      .Select(name => name!.Trim())).CleanTrim();
  }
}
