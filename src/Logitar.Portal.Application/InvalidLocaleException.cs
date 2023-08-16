using FluentValidation.Results;

namespace Logitar.Portal.Application;

public class InvalidLocaleException : Exception
{
  private const string ErrorMessage = "The specified locale is not valid.";

  public InvalidLocaleException(string cultureName, string propertyName, Exception innerException)
    : base(BuildMessage(cultureName, propertyName), innerException)
  {
    CultureName = cultureName;
    PropertyName = propertyName;
  }

  public string CultureName
  {
    get => (string)Data[nameof(CultureName)]!;
    private set => Data[nameof(CultureName)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, CultureName)
  {
    ErrorCode = "InvalidLocale"
  };

  private static string BuildMessage(string cultureName, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("CultureName: ").AppendLine(cultureName);
    message.Append("PropertyName: ").AppendLine(propertyName);

    return message.ToString();
  }
}
