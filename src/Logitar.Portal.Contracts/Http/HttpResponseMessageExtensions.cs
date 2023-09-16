namespace Logitar.Portal.Contracts.Http;

public static class HttpResponseMessageExtensions
{
  public static async Task<HttpResponseDetail> DetailAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default)
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
