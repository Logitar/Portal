using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Application.Senders;

public class NoDefaultSenderException : Exception
{
  public const string ErrorMessage = "The specified tenant has no default sender.";

  public TenantId? TenantId
  {
    get => TenantId.TryCreate((string?)Data[nameof(TenantId)]);
    private set => Data[nameof(TenantId)] = value?.Value;
  }

  public NoDefaultSenderException(TenantId? tenantId) : base(BuildMessage(tenantId))
  {
    TenantId = tenantId;
  }

  private static string BuildMessage(TenantId? tenantId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), tenantId?.Value, "<null>")
    .Build();
}
