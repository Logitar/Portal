using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Senders;

public class SenderNotFoundException : Exception
{
  public const string ErrorMessage = "The specified sender could not be found.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
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
    RealmId = senderId.TenantId?.ToGuid();
    SenderId = senderId.EntityId.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(SenderId senderId, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), senderId.TenantId?.ToGuid())
    .AddData(nameof(SenderId), senderId.EntityId.ToGuid())
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
