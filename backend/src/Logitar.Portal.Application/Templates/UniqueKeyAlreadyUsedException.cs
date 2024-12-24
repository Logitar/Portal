using Logitar.Identity.Core;

namespace Logitar.Portal.Application.Templates;

public class UniqueKeyAlreadyUsedException : Exception
{
  public const string ErrorMessage = "The specified unique key is already used.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public string UniqueKey
  {
    get => (string)Data[nameof(UniqueKey)]!;
    private set => Data[nameof(UniqueKey)] = value;
  }

  public UniqueKeyAlreadyUsedException(TenantId? tenantId, Identifier uniqueKey) : base(BuildMessage(tenantId, uniqueKey))
  {
    RealmId = tenantId?.ToGuid();
    UniqueKey = uniqueKey.Value;
  }

  private static string BuildMessage(TenantId? tenantId, Identifier uniqueKey) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), tenantId?.ToGuid(), "<null>")
    .AddData(nameof(UniqueKey), uniqueKey)
    .Build();
}
