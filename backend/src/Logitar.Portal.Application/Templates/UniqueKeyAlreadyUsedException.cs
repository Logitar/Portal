using Logitar.Identity.Core;

namespace Logitar.Portal.Application.Templates;

public class UniqueKeyAlreadyUsedException : Exception
{
  public const string ErrorMessage = "The specified unique key is already used.";

  public string? TenantId
  {
    get => (string?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }
  public string UniqueKey
  {
    get => (string)Data[nameof(UniqueKey)]!;
    private set => Data[nameof(UniqueKey)] = value;
  }

  public UniqueKeyAlreadyUsedException(TenantId? tenantId, Identifier uniqueKey) : base(BuildMessage(tenantId, uniqueKey))
  {
    TenantId = tenantId?.Value;
    UniqueKey = uniqueKey.Value;
  }

  private static string BuildMessage(TenantId? tenantId, Identifier uniqueKey) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), tenantId, "<null>")
    .AddData(nameof(UniqueKey), uniqueKey)
    .Build();
}
