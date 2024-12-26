using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Domain.Messages;

public record TemplateSummary
{
  public TemplateId Id { get; }
  public UniqueKey UniqueKey { get; }
  public DisplayNameUnit? DisplayName { get; }

  [JsonConstructor]
  public TemplateSummary(TemplateId id, UniqueKey uniqueKey, DisplayNameUnit? displayName)
  {
    Id = id;
    UniqueKey = uniqueKey;
    DisplayName = displayName;
  }

  public TemplateSummary(Template template) : this(template.Id, template.UniqueKey, template.DisplayName)
  {
  }
}
