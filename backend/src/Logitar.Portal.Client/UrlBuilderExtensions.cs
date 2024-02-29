using Logitar.Net.Http;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Client;

internal static class UrlBuilderExtensions
{
  private const char SortSeparator = '.';
  private const string IsDescending = "DESC";
  private const string VersionQuery = "version";

  public static IUrlBuilder SetQuery(this IUrlBuilder builder, SearchPayload payload)
  {
    foreach (Guid id in payload.Ids)
    {
      builder.AddQuery("ids", id.ToString());
    }

    if (payload.Search.Terms.Count > 0)
    {
      foreach (SearchTerm term in payload.Search.Terms)
      {
        builder.AddQuery("search_terms", term.Value);
      }
      builder.SetQuery("search_operator", payload.Search.Operator.ToString());
    }

    foreach (SortOption sort in payload.Sort)
    {
      if (sort.IsDescending)
      {
        builder.AddQuery("sort", string.Join(SortSeparator, IsDescending, sort.Field));
      }
      else
      {
        builder.AddQuery("sort", sort.Field);
      }
    }

    if (payload.Skip > 0)
    {
      builder.SetQuery("skip", payload.Skip.ToString());
    }
    if (payload.Limit > 0)
    {
      builder.SetQuery("limit", payload.Limit.ToString());
    }

    return builder;
  }

  public static IUrlBuilder SetVersion(this IUrlBuilder builder, long? version)
  {
    return version.HasValue ? builder.SetQuery(VersionQuery, version.Value.ToString()) : builder;
  }
}
