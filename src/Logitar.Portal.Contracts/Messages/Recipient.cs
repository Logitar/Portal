using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Messages;

public record Recipient
{
  public RecipientType Type { get; set; }
  public User? User { get; set; }

  public string Address { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
}
