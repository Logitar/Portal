namespace Logitar.Portal.Contracts.Messages;

public record SendDemoMessagePayload
{
  public Guid? SenderId { get; set; }
  public Guid TemplateId { get; set; }

  public string? Locale { get; set; }

  public IEnumerable<Variable> Variables { get; set; } = Enumerable.Empty<Variable>();
}
