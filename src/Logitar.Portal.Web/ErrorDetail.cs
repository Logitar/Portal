namespace Logitar.Portal.Web;

internal record ErrorDetail
{
  public ErrorDetail(Exception exception, string? message = null)
    : this(exception.GetType().Name.Remove(nameof(Exception)), message ?? exception.Message)
  {
  }
  public ErrorDetail(string code, string message)
  {
    ErrorCode = code;
    ErrorMessage = message;
  }

  public string ErrorCode { get; }
  public string ErrorMessage { get; }
}
