using Logitar.Identity.Core;
using Logitar.Identity.Core.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys;

public class ApiKeyNotFoundException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified API key could not be found.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid ApiKeyId
  {
    get => (Guid)Data[nameof(ApiKeyId)]!;
    private set => Data[nameof(ApiKeyId)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public ApiKeyNotFoundException(ApiKeyId id, string? propertyName = null) : base(BuildMessage(id, propertyName))
  {
    RealmId = id.TenantId?.ToGuid();
    ApiKeyId = id.EntityId.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(ApiKeyId id, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), id.TenantId?.ToGuid())
    .AddData(nameof(ApiKeyId), id.EntityId.ToGuid())
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
