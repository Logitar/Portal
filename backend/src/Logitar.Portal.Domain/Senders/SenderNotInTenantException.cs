using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Domain.Senders;

public class SenderNotInTenantException : Exception
{
  public const string ErrorMessage = "The specified sender is not in the specified tenant.";

  public SenderId SenderId
  {
    get => new((string)Data[nameof(SenderId)]!);
    private set => Data[nameof(SenderId)] = value.Value;
  }
  public TenantId? ExpectedTenantId
  {
    get => TenantId.TryCreate((string?)Data[nameof(ExpectedTenantId)]);
    private set => Data[nameof(ExpectedTenantId)] = value?.Value;
  }
  public TenantId? ActualTenantId
  {
    get => TenantId.TryCreate((string?)Data[nameof(ActualTenantId)]);
    private set => Data[nameof(ActualTenantId)] = value?.Value;
  }

  public SenderNotInTenantException(Sender sender, TenantId? expectedTenant) : base(BuildMessage(sender, expectedTenant))
  {
    SenderId = sender.Id;
    ExpectedTenantId = expectedTenant;
    ActualTenantId = sender.TenantId;
  }

  private static string BuildMessage(Sender sender, TenantId? expectedTenant) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(SenderId), sender.Id.Value)
    .AddData(nameof(ExpectedTenantId), expectedTenant?.Value, "<null>")
    .AddData(nameof(ActualTenantId), sender.TenantId?.Value, "<null>")
    .Build();
}
