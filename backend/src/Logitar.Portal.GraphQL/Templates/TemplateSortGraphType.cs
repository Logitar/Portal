using GraphQL.Types;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.GraphQL.Templates;

internal class TemplateSortGraphType : EnumerationGraphType<TemplateSort>
{
  public TemplateSortGraphType()
  {
    Name = nameof(TemplateSort);
    Description = "Represents the available template fields for sorting.";

    Add(TemplateSort.DisplayName, "The templates will be sorted by their display name.");
    Add(TemplateSort.Subject, "The templates will be sorted by their subject.");
    Add(TemplateSort.UniqueKey, "The templates will be sorted by their unique key.");
    Add(TemplateSort.UpdatedOn, "The templates will be sorted by their latest update date and time.");
  }

  private void Add(TemplateSort value, string description) => Add(value.ToString(), value, description);
}
