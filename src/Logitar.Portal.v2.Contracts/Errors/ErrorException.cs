namespace Logitar.Portal.v2.Contracts.Errors;

public class ErrorException : Exception
{
  public ErrorException(Error error, Exception? innerException = null) : base(error?.ToString(), innerException)
  {
    Data[nameof(Error)] = error;
  }

  public Error Error => (Error)Data[nameof(Error)]!;
}
