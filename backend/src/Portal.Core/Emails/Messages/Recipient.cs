using Portal.Core.Users;
using System.Text.Json.Serialization;

namespace Portal.Core.Emails.Messages
{
  public class Recipient
  {
    public Recipient(string address, string? displayName, RecipientType type = RecipientType.To)
    {
      Address = address ?? throw new ArgumentNullException(nameof(address));
      DisplayName = displayName;

      Type = type;
    }

    public RecipientType Type { get; private set; }

    public string Address { get; private set; }
    public string? DisplayName { get; private set; }

    public Guid? UserId { get; private set; }
    public string? Username { get; private set; }

    [JsonIgnore]
    public User? User { get; private set; }

    public static Recipient FromUser(User user, RecipientType type = RecipientType.To)
    {
      ArgumentNullException.ThrowIfNull(user);

      if (user.Email == null)
      {
        throw new ArgumentException($"The {user.Email} is required.", nameof(user));
      }

      return new Recipient(user.Email, user.FullName, type)
      {
        UserId = user.Id,
        Username = user.Username
      };
    }

    public override string ToString() => DisplayName == null
      ? Address
      : $"{DisplayName} <{Address}>";
  }
}
