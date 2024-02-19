using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Templates;

public record SearchTemplatesPayload : SearchPayload
{
  public string? ContentType { get; set; }

  public new List<TemplateSortOption> Sort { get; set; } = [];
}
