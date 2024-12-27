using Logitar.Identity.Core;

namespace Logitar.Portal.Domain.Senders;

public class SenderNotInTenantException : Exception
{
  public const string ErrorMessage = "The specified sender is not in the specified tenant.";

  public Guid SenderId
  {
    get => (Guid)Data[nameof(SenderId)]!;
    private set => Data[nameof(SenderId)] = value;
  }
  public Guid? ExpectedTenantId
  {
    get => (Guid?)Data[nameof(ExpectedTenantId)];
    private set => Data[nameof(ExpectedTenantId)] = value;
  }
  public Guid? ActualTenantId
  {
    get => (Guid?)Data[nameof(ActualTenantId)];
    private set => Data[nameof(ActualTenantId)] = value;
  }

  public SenderNotInTenantException(Sender sender, TenantId? expectedTenant) : base(BuildMessage(sender, expectedTenant))
  {
    SenderId = sender.EntityId.ToGuid();
    ExpectedTenantId = expectedTenant?.ToGuid();
    ActualTenantId = sender.TenantId?.ToGuid();
  }

  private static string BuildMessage(Sender sender, TenantId? expectedTenant) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(SenderId), sender.EntityId.ToGuid())
    .AddData(nameof(ExpectedTenantId), expectedTenant?.ToGuid(), "<null>")
    .AddData(nameof(ActualTenantId), sender.TenantId?.ToGuid(), "<null>")
    .Build();
}
