using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Models.Search;

public record SearchModel
{
  protected const char SortSeparator = '.';
  protected const string IsDescending = "DESC";

  [FromQuery(Name = "ids")]
  public List<Guid> Ids { get; set; } = [];

  [FromQuery(Name = "search_terms")]
  public List<string> SearchTerms { get; set; } = [];

  [FromQuery(Name = "search_operator")]
  public SearchOperator SearchOperator { get; set; }

  [FromQuery(Name = "sort")]
  public List<string> Sort { get; set; } = [];

  [FromQuery(Name = "skip")]
  public int Skip { get; set; }

  [FromQuery(Name = "limit")]
  public int Limit { get; set; }
}
