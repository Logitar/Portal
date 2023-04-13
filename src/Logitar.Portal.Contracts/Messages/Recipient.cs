namespace Logitar.Portal.Contracts.Messages;

public record Recipient
{
  public RecipientType Type { get; set; }

  public string Address { get; set; } = string.Empty;
  public string? DisplayName { get; set; }

  public Guid? UserId { get; set; }
  public string? Username { get; set; }
}
