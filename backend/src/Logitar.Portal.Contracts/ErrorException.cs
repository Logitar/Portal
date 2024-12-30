namespace Logitar.Portal.Contracts;

public class ErrorException : Exception
{
  public Error Error { get; }

  public ErrorException(Error error) : base(error.Message)
  {
    Error = error;
  }
}
