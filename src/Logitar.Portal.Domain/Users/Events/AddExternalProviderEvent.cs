namespace Logitar.Portal.Domain.Users.Events
{
  public class AddExternalProviderEvent : EventBase
  {
    public AddExternalProviderEvent(string key, string value, string? displayName, Guid userId) : base(userId)
    {
      Key = key ?? throw new ArgumentNullException(nameof(key));
      Value = value ?? throw new ArgumentNullException(nameof(value));
      DisplayName = displayName;
    }

    public string Key { get; }
    public string Value { get; }

    public string? DisplayName { get; }
  }
}
