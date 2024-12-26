using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Application.Realms;

public class UniqueKeyAlreadyUsedException : Exception
{
  public const string ErrorMessage = "The specified unique key is already used.";

  public TenantId? TenantId
  {
    get => TenantId.TryCreate((string?)Data[nameof(TenantId)]);
    private set => Data[nameof(TenantId)] = value?.Value;
  }
  public UniqueKey UniqueKey
  {
    get => new((string)Data[nameof(UniqueKey)]!);
    private set => Data[nameof(UniqueKey)] = value.Value;
  }

  public UniqueKeyAlreadyUsedException(TenantId? tenantId, UniqueKey uniqueKey) : base(BuildMessage(tenantId, uniqueKey))
  {
    TenantId = tenantId;
    UniqueKey = uniqueKey;
  }

  private static string BuildMessage(TenantId? tenantId, UniqueKey uniqueKey) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), tenantId?.Value, "<null>")
    .AddData(nameof(UniqueKey), uniqueKey.Value)
    .Build();
}
