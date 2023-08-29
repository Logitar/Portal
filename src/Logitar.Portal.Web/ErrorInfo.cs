namespace Logitar.Portal.Web;

internal record ErrorInfo
{
  public ErrorInfo(Exception exception, string? message = null)
  {
    ErrorCode = exception.GetType().Name.Remove(nameof(Exception));
    ErrorMessage = message ?? exception.Message;
  }

  public string ErrorCode { get; }
  public string ErrorMessage { get; }
}
