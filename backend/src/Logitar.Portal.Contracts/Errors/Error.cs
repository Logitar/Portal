namespace Logitar.Portal.Contracts.Errors;

public record Error
{
  public string Code { get; set; }
  public string Message { get; set; }

  public Error() : this(string.Empty, string.Empty)
  {
  }

  public Error(string code, string message)
  {
    Code = code;
    Message = message;
  }
}
