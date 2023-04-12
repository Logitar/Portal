namespace Logitar.Portal.Domain.Users.Events
{
  public class ChangedPasswordEvent : EventBase
  {
    public ChangedPasswordEvent(string passwordHash, Guid userId) : base(userId)
    {
      PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
    }

    public string PasswordHash { get; private set; }
  }
}
