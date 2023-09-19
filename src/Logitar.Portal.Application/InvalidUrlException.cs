using FluentValidation.Results;
using Logitar.Portal.Domain;

namespace Logitar.Portal.Application;

public class InvalidUrlException : Exception, IFailureException
{
  private const string ErrorMessage = "The specified URL is not valid.";

  public InvalidUrlException(string uriString, string propertyName, Exception? innerException = null)
    : base(BuildMessage(uriString, propertyName), innerException)
  {
    UriString = uriString;
    PropertyName = propertyName;
  }

  public string UriString
  {
    get => (string)Data[nameof(UriString)]!;
    private set => Data[nameof(UriString)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, UriString)
  {
    ErrorCode = "InvalidUrl"
  };

  private static string BuildMessage(string uriString, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("UriString: ").AppendLine(uriString);
    message.Append("PropertyName: ").AppendLine(propertyName);

    return message.ToString();
  }
}
