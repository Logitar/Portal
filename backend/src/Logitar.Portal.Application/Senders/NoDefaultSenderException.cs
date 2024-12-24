using Logitar.Identity.Core;

namespace Logitar.Portal.Application.Senders;

public class NoDefaultSenderException : Exception
{
  public const string ErrorMessage = "The specified tenant has no default sender.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }

  public NoDefaultSenderException(TenantId? tenantId) : base(BuildMessage(tenantId))
  {
    RealmId = tenantId?.ToGuid();
  }

  private static string BuildMessage(TenantId? tenantId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), tenantId?.ToGuid(), "<null>")
    .Build();
}
