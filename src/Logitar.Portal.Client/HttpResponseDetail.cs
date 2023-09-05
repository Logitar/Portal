namespace Logitar.Portal.Client;

public record HttpResponseDetail
{
  public string? Content { get; init; }
  public string? ReasonPhrase { get; init; }
  public int StatusCode { get; init; }
  public string StatusText { get; init; } = string.Empty;
  public string Version { get; init; } = string.Empty;

  internal static async Task<HttpResponseDetail> CreateAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
  {
    string? content = null;
    try
    {
      content = await response.Content.ReadAsStringAsync(cancellationToken);
    }
    catch (Exception)
    {
    }

    return new HttpResponseDetail
    {
      Content = content,
      ReasonPhrase = response.ReasonPhrase,
      StatusCode = (int)response.StatusCode,
      StatusText = response.StatusCode.ToString(),
      Version = response.Version.ToString()
    };
  }
}
