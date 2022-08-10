namespace Logitar.Portal.Core.Emails.Messages.Models
{
  public class MessageSummary : AggregateSummary
  {
    public bool IsDemo { get; set; }

    public string Subject { get; set; } = null!;
    public int Recipients { get; set; }

    public string SenderAddress { get; set; } = null!;
    public string? SenderDisplayName { get; set; }

    public string TemplateKey { get; set; } = null!;
    public string? TemplateDisplayName { get; set; }

    public bool HasErrors { get; set; }
    public bool Succeeded { get; set; }
  }
}
