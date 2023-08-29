using FluentValidation.Results;

namespace Logitar.Portal.Application;

public class InvalidGenderException : Exception
{
  private const string ErrorMessage = "The specified gender is not valid.";

  public InvalidGenderException(string value, string propertyName, Exception? innerException = null)
    : base(BuildMessage(value, propertyName), innerException)
  {
    Value = value;
    PropertyName = propertyName;
  }

  public string Value
  {
    get => (string)Data[nameof(Value)]!;
    private set => Data[nameof(Value)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, Value)
  {
    ErrorCode = "InvalidGender"
  };

  private static string BuildMessage(string value, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("Value: ").AppendLine(value);
    message.Append("PropertyName: ").AppendLine(propertyName);

    return message.ToString();
  }
}
