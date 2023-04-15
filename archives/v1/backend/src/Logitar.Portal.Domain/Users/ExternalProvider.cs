using Logitar.Portal.Core;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Domain.Users
{
  public class ExternalProvider
  {
    public ExternalProvider(User user, string key, string value, DateTime addedAt, Guid addedById, string? displayName = null)
    {
      Id = Guid.NewGuid();

      AddedAt = addedAt;
      AddedById = addedById;

      User = user;
      UserSid = user.Sid;

      Realm = user.Realm ?? throw new ArgumentException($"The {nameof(user.Realm)} is required.", nameof(user));
      RealmSid = user.Realm.Sid;

      Key = key ?? throw new ArgumentNullException(nameof(key));
      Value = value ?? throw new ArgumentNullException(nameof(value));

      DisplayName = displayName?.CleanTrim();
    }
    private ExternalProvider()
    {
    }

    public Guid Id { get; private set; }
    public int Sid { get; private set; }

    public DateTime AddedAt { get; private set; }
    public Guid AddedById { get; private set; }

    public Realm? Realm { get; private set; }
    public int RealmSid { get; private set; }

    public User? User { get; private set; }
    public int UserSid { get; private set; }

    public string Key { get; private set; } = null!;
    public string Value { get; private set; } = null!;

    public string? DisplayName { get; private set; }

    public override bool Equals(object? obj) => obj is ExternalProvider provider
      && provider.Key == Key
      && provider.Value == Value;
    public override int GetHashCode() => HashCode.Combine(Key, Value);
    public override string ToString() => string.Join('=', Key, Value);
  }
}
