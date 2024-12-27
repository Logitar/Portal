using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Senders;

public class CannotDeleteDefaultSenderException : Exception
{
  private const string ErrorMessage = "The default sender cannot be deleted, unless its the only sender in its Realm.";

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

  public CannotDeleteDefaultSenderException(Sender sender) : base(BuildMessage(sender))
  {
    TenantId = sender.TenantId?.ToGuid();
    SenderId = sender.EntityId.ToGuid();
  }

  private static string BuildMessage(Sender sender) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), sender.TenantId?.ToGuid(), "<null>")
    .AddData(nameof(SenderId), sender.EntityId.ToGuid())
    .Build();
}
