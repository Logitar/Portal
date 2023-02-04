using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Users;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Domain.Messages
{
  public record Recipient
  {
    public Recipient(string address, string? displayName = null, RecipientType type = RecipientType.To)
      : this(type, address, displayName)
    {
    }
    public Recipient(User user, RecipientType type = RecipientType.To)
      : this(type, user.Email ?? string.Empty, user.FullName, user.Id, user.Username, user.Locale)
    {
      User = user;
    }

    [JsonConstructor]
    public Recipient(RecipientType type, string address, string? displayName = null, AggregateId? userId = null, string? username = null, CultureInfo? userLocale = null)
    {
      Type = type;

      Address = address;
      DisplayName = displayName;

      UserId = userId;
      Username = username;
      UserLocale = userLocale;
    }



    public RecipientType Type { get; }

    public string Address { get; }
    public string? DisplayName { get; }

    public AggregateId? UserId { get; }
    public string? Username { get; }
    public CultureInfo? UserLocale { get; }

    [JsonIgnore]
    public User? User { get; private set; }
  }
}
