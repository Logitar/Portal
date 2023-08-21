using FluentValidation.Results;

namespace Logitar.Portal.Application;

public class InvalidTimeZoneEntryException : Exception
{
  private const string ErrorMessage = "The specified time zone entry is not valid.";

  public InvalidTimeZoneEntryException(string id, string propertyName, Exception innerException)
    : base(BuildMessage(id, propertyName), innerException)
  {
    Id = id;
    PropertyName = propertyName;
  }

  public string Id
  {
    get => (string)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, Id)
  {
    ErrorCode = "InvalidTimeZoneEntry"
  };

  private static string BuildMessage(string id, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("Id: ").AppendLine(id);
    message.Append("PropertyName: ").AppendLine(propertyName);

    return message.ToString();
  }
}
