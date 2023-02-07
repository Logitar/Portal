namespace Logitar.Portal.Contracts.Messages
{
  public record RecipientModel
  {
    public Guid Id { get; set; }

    public RecipientType Type { get; set; }

    public string Address { get; set; } = string.Empty;
    public string? DisplayName { get; set; }

    public string? UserId { get; set; }
    public string? Username { get; set; }
    public string? UserLocale { get; set; }
  }
}
