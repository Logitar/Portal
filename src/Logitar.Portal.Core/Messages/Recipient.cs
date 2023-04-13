using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Core.Users;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Core.Messages;

public record Recipient
{
  public RecipientType Type { get; init; }

  public string Address { get; init; } = string.Empty;
  public string? DisplayName { get; init; }

  public Guid? UserId { get; init; }
  public string? UserLocale { get; init; }
  public string? Username { get; init; }

  [JsonIgnore]
  public UserAggregate? User { get; init; }

  public static Recipient From(RecipientInput input, UserAggregate? user = null) => new()
  {
    Type = input.Type,
    Address = user?.Email?.Address ?? input.Address ?? string.Empty,
    DisplayName = user?.FullName ?? input.DisplayName,
    UserId = user?.Id.ToGuid(),
    UserLocale = user?.Locale?.Name,
    Username = user?.Username,
    User = user
  };
}
