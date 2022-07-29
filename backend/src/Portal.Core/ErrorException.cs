namespace Portal.Core
{
  public class ErrorException : Exception
  {
    public ErrorException(Error error, Exception? innerException = null)
      : base(error?.ToString(), innerException)
    {
      Error = error ?? throw new ArgumentNullException(nameof(error));
    }

    public Error Error { get; }
  }
}
