using Logitar.Net.Http;

namespace Logitar.Portal.Client;

internal static class UrlBuilderExtensions
{
  private const string VersionQuery = "version";

  public static IUrlBuilder SetVersion(this IUrlBuilder builder, long? version)
  {
    return version.HasValue ? builder.SetQuery(VersionQuery, version.Value.ToString()) : builder;
  }
}
