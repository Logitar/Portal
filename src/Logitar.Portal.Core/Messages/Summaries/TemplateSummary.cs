using Logitar.EventSourcing;
using Logitar.Portal.Core.Templates;

namespace Logitar.Portal.Core.Messages.Summaries;

public record TemplateSummary
{
  public AggregateId Id { get; init; }

  public string UniqueName { get; init; } = string.Empty;
  public string? DisplayName { get; init; }

  public string ContentType { get; init; } = string.Empty;

  public static TemplateSummary From(TemplateAggregate template) => new()
  {
    Id = template.Id,
    UniqueName = template.UniqueName,
    DisplayName = template.DisplayName,
    ContentType = template.ContentType
  };
}
