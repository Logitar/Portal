using FluentValidation.Results;

namespace Logitar.Portal.Application;

public class InvalidLocaleException : Exception
{
  private const string ErrorMessage = "The specified locale is not valid.";

  public InvalidLocaleException(string code, string propertyName, Exception? innerException = null)
    : base(BuildMessage(code, propertyName), innerException)
  {
    Code = code;
    PropertyName = propertyName;
  }

  public string Code
  {
    get => (string)Data[nameof(Code)]!;
    private set => Data[nameof(Code)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, Code)
  {
    ErrorCode = "InvalidLocale"
  };

  private static string BuildMessage(string code, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("Code: ").AppendLine(code);
    message.Append("PropertyName: ").AppendLine(propertyName);

    return message.ToString();
  }
}
