namespace Logitar.Portal.Contracts.Http;

public class HttpFailureException : Exception
{
  public HttpFailureException(HttpResponseDetail detail, Exception innerException)
    : base("The remote API did not return a success status code.", innerException)
  {
    Detail = detail;
  }

  public HttpResponseDetail Detail
  {
    get => (HttpResponseDetail)Data[nameof(Detail)]!;
    private set => Data[nameof(Detail)] = value;
  }
}
