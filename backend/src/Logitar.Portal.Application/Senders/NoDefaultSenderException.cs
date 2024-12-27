using Logitar.Identity.Core;

namespace Logitar.Portal.Application.Senders;

public class NoDefaultSenderException : Exception
{
  private const string ErrorMessage = "The specified tenant has no default sender.";

  public Guid? TenantId
  {
    get => (Guid?)Data[nameof(TenantId)]!;
    private set => Data[nameof(TenantId)] = value;
  }

  public NoDefaultSenderException(TenantId? tenantId) : base(BuildMessage(tenantId))
  {
    TenantId = tenantId?.ToGuid();
  }

  private static string BuildMessage(TenantId? tenantId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), tenantId?.ToGuid(), "<null>")
    .Build();
}
