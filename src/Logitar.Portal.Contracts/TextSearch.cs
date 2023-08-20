namespace Logitar.Portal.Contracts;

public record TextSearch
{
  public TextSearch() : this(Enumerable.Empty<SearchTerm>())
  {
  }
  public TextSearch(IEnumerable<SearchTerm> terms, ConditionOperator @operator = ConditionOperator.And)
  {
    Terms = terms;
    Operator = @operator;
  }

  public IEnumerable<SearchTerm> Terms { get; set; }
  public ConditionOperator Operator { get; set; }
}
