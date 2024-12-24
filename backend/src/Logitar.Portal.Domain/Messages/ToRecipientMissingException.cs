using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Domain.Messages;

public class ToRecipientMissingException : Exception
{
  public const string ErrorMessage = $"At least one {nameof(RecipientType.To)} recipient must be provided.";

  public string MessageId
  {
    get => (string)Data[nameof(MessageId)]!;
    private set => Data[nameof(MessageId)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public ToRecipientMissingException(Message message, string? propertyName = null) : base(BuildMessage(message, propertyName))
  {
    MessageId = message.Id.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Message message, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(MessageId), message.Id)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
