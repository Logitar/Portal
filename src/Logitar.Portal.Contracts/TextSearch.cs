namespace Logitar.Portal.Contracts;

public record TextSearch
{
  public TextSearch() : this(Enumerable.Empty<SearchTerm>())
  {
  }
  public TextSearch(IEnumerable<SearchTerm> terms, SearchOperator @operator = SearchOperator.And)
  {
    Terms = terms;
    Operator = @operator;
  }

  public IEnumerable<SearchTerm> Terms { get; set; } = Enumerable.Empty<SearchTerm>();
  public SearchOperator Operator { get; set; }
}
