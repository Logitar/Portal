using System.Net;

namespace Portal.Core
{
  public class ApiException : Exception
  {
    public ApiException(
      HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
      string? message = null,
      Exception? innerException = null
    ) : base(message ?? $"{(int)statusCode} {statusCode}", innerException)
    {
      StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
    public object? Value { get; protected set; }
  }
}
