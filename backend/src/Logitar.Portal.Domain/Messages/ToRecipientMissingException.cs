using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Domain.Messages;

public class ToRecipientMissingException : Exception
{
  public const string ErrorMessage = $"At least one {nameof(RecipientType.To)} recipient must be provided.";

  public MessageId MessageId
  {
    get => new((string)Data[nameof(MessageId)]!);
    private set => Data[nameof(MessageId)] = value.Value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public ToRecipientMissingException(MessageAggregate message, string? propertyName = null) : base(BuildMessage(message, propertyName))
  {
    MessageId = message.Id;
    PropertyName = propertyName;
  }

  private static string BuildMessage(MessageAggregate message, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(MessageId), message.Id.Value)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
