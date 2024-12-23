using Logitar.Identity.Core;

namespace Logitar.Portal.Application.Senders;

public class NoDefaultSenderException : Exception
{
  public const string ErrorMessage = "The specified tenant has no default sender.";

  public string? TenantId
  {
    get => (string?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }

  public NoDefaultSenderException(TenantId? tenantId) : base(BuildMessage(tenantId))
  {
    TenantId = tenantId?.Value;
  }

  private static string BuildMessage(TenantId? tenantId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), tenantId, "<null>")
    .Build();
}
