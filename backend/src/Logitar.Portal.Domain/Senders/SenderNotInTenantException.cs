using Logitar.Identity.Core;

namespace Logitar.Portal.Domain.Senders;

public class SenderNotInTenantException : Exception
{
  public const string ErrorMessage = "The specified sender is not in the specified tenant.";

  public string SenderId
  {
    get => (string)Data[nameof(SenderId)]!;
    private set => Data[nameof(SenderId)] = value;
  }
  public string? ExpectedTenantId
  {
    get => (string?)Data[nameof(ExpectedTenantId)];
    private set => Data[nameof(ExpectedTenantId)] = value;
  }
  public string? ActualTenantId
  {
    get => (string?)Data[nameof(ActualTenantId)];
    private set => Data[nameof(ActualTenantId)] = value;
  }

  public SenderNotInTenantException(SenderAggregate sender, TenantId? expectedTenant) : base(BuildMessage(sender, expectedTenant))
  {
    SenderId = sender.Id.Value;
    ExpectedTenantId = expectedTenant?.Value;
    ActualTenantId = sender.TenantId?.Value;
  }

  private static string BuildMessage(SenderAggregate sender, TenantId? expectedTenant) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(SenderId), sender.Id)
    .AddData(nameof(ExpectedTenantId), expectedTenant, "<null>")
    .AddData(nameof(ActualTenantId), sender.TenantId, "<null>")
    .Build();
}
