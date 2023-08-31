namespace Logitar.Portal.Contracts.ApiKeys;

public record ApiKeySortOption : SortOption
{
  public ApiKeySortOption() : this(ApiKeySort.UpdatedOn, isDescending: true)
  {
  }
  public ApiKeySortOption(ApiKeySort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
    Field = field;
  }

  public new ApiKeySort Field { get; set; }
}
