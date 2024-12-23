using Logitar.Identity.Core;
using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Domain.Messages;

public record TemplateSummary
{
  public TemplateId Id { get; }
  public Identifier UniqueKey { get; }
  public DisplayName? DisplayName { get; }

  [JsonConstructor]
  public TemplateSummary(TemplateId id, Identifier uniqueKey, DisplayName? displayName)
  {
    Id = id;
    UniqueKey = uniqueKey;
    DisplayName = displayName;
  }

  public TemplateSummary(Template template) : this(template.Id, template.UniqueKey, template.DisplayName)
  {
  }
}
