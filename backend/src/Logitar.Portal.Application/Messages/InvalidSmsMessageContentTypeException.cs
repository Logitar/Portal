namespace Logitar.Portal.Application.Messages;

public class InvalidSmsMessageContentTypeException : Exception
{
  public const string ErrorMessage = "A SMS message cannot be sent using the specified content type.";

  public string ContentType
  {
    get => (string)Data[nameof(ContentType)]!;
    private set => Data[nameof(ContentType)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public InvalidSmsMessageContentTypeException(string contentType, string? propertyName = null) : base(BuildMessage(contentType, propertyName))
  {
    ContentType = contentType;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string contentType, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ContentType), contentType)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
