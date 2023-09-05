namespace Logitar.Portal.Contracts;

public record ErrorDetail
{
  public ErrorDetail(string errorCode, string errorMessage)
  {
    ErrorCode = errorCode;
    ErrorMessage = errorMessage;
  }

  public string ErrorCode { get; }
  public string ErrorMessage { get; }

  public static ErrorDetail From(Exception exception, string? message = null)
  {
    return new ErrorDetail(errorCode: exception.GetType().Name.Replace(nameof(Exception), string.Empty),
      errorMessage: message ?? exception.Message);
  }
}
