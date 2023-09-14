namespace Logitar.Portal.Contracts.Templates;

public record SearchTemplatesPayload : SearchPayload
{
  public string? Realm { get; set; }

  public string? ContentType { get; set; }

  public new IEnumerable<TemplateSortOption> Sort { get; set; } = Enumerable.Empty<TemplateSortOption>();
}
