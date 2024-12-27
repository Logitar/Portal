using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Domain.Messages;

public class ToRecipientMissingException : Exception
{
  private const string ErrorMessage = $"At least one {nameof(RecipientType.To)} recipient must be provided.";

  public Guid? TenantId
  {
    get => (Guid?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }
  public Guid MessageId
  {
    get => (Guid)Data[nameof(MessageId)]!;
    private set => Data[nameof(MessageId)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public ToRecipientMissingException(Message message, string? propertyName = null) : base(BuildMessage(message, propertyName))
  {
    TenantId = message.TenantId?.ToGuid();
    MessageId = message.EntityId.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(Message message, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), message.TenantId?.ToGuid())
    .AddData(nameof(MessageId), message.EntityId.ToGuid())
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
