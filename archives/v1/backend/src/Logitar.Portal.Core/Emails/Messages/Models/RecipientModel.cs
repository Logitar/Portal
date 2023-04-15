namespace Logitar.Portal.Core.Emails.Messages.Models
{
  public class RecipientModel
  {
    public RecipientType Type { get; set; }

    public string Address { get; set; } = null!;
    public string? DisplayName { get; set; }

    public Guid? UserId { get; set; }
    public string? Username { get; set; }
  }
}
