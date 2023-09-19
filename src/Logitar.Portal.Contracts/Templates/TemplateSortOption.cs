namespace Logitar.Portal.Contracts.Templates;

public record TemplateSortOption : SortOption
{
  public TemplateSortOption() : this(TemplateSort.UpdatedOn, isDescending: true)
  {
  }
  public TemplateSortOption(TemplateSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
    Field = field;
  }

  public new TemplateSort Field { get; set; }
}
