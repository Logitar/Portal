namespace Logitar.Portal.Contracts;

public record TextSearch
{
  public TextSearch() : this(Enumerable.Empty<SearchTerm>())
  {
  }
  public TextSearch(IEnumerable<SearchTerm> terms, QueryOperator @operator = QueryOperator.And)
  {
    Terms = terms;
    Operator = @operator;
  }

  public IEnumerable<SearchTerm> Terms { get; set; }
  public QueryOperator Operator { get; set; }
}
