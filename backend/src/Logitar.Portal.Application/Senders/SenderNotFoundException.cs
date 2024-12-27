using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Senders;

public class SenderNotFoundException : Exception
{
  private const string ErrorMessage = "The specified sender could not be found.";

  public Guid? TenantId
  {
    get => (Guid?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }
  public Guid SenderId
  {
    get => (Guid)Data[nameof(SenderId)]!;
    private set => Data[nameof(SenderId)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public SenderNotFoundException(SenderId senderId, string? propertyName = null) : base(BuildMessage(senderId, propertyName))
  {
    TenantId = senderId.TenantId?.ToGuid();
    SenderId = senderId.EntityId.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(SenderId senderId, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), senderId.TenantId?.ToGuid(), "<null>")
    .AddData(nameof(SenderId), senderId.EntityId.ToGuid())
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
