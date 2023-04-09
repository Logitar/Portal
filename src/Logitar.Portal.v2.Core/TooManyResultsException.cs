namespace Logitar.Portal.v2.Core;

public class TooManyResultsException : Exception
{
  public TooManyResultsException() : base("Too many results have been found.")
  {
  }
}
