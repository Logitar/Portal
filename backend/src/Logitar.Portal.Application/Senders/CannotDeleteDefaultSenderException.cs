using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Senders;

public class CannotDeleteDefaultSenderException : Exception
{
  public const string ErrorMessage = "The default sender cannot be deleted, unless its the only sender in its Realm.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)]!;
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid SenderId
  {
    get => (Guid)Data[nameof(SenderId)]!;
    private set => Data[nameof(SenderId)] = value;
  }

  public CannotDeleteDefaultSenderException(Sender sender) : base(BuildMessage(sender))
  {
    RealmId = sender.Id.TenantId?.ToGuid();
    SenderId = sender.Id.EntityId.ToGuid();
  }

  private static string BuildMessage(Sender sender) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), sender.Id.TenantId?.ToGuid())
    .AddData(nameof(SenderId), sender.Id.EntityId.ToGuid())
    .Build();
}
