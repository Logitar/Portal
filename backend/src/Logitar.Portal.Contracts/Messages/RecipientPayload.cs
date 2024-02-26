namespace Logitar.Portal.Contracts.Messages;

public record RecipientPayload
{
  public RecipientType Type { get; set; }
  public Guid? UserId { get; set; }

  public string? Address { get; set; }
  public string? DisplayName { get; set; }
}
