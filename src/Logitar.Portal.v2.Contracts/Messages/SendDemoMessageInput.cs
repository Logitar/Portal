namespace Logitar.Portal.v2.Contracts.Messages;

public record SendDemoMessageInput
{
  public Guid TemplateId { get; set; }

  public string? Locale { get; set; }

  public IEnumerable<Variable>? Variables { get; set; } // TODO(fpion): Value is now required
}
