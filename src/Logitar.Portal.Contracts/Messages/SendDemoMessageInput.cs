namespace Logitar.Portal.Contracts.Messages;

public record SendDemoMessageInput
{
  public Guid TemplateId { get; set; }

  public string? Locale { get; set; }

  public IEnumerable<Variable>? Variables { get; set; }
}
