namespace Logitar.Portal.Contracts
{
  public class ErrorException : Exception
  {
    public ErrorException(Error error, Exception? innerException = null)
      : base(error.ToString(), innerException)
    {
      Data["Error"] = error;
      Error = error;
    }

    public Error Error { get; }
  }
}
