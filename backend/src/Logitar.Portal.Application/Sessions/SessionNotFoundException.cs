using Logitar.Identity.Core;
using Logitar.Identity.Core.Sessions;

namespace Logitar.Portal.Application.Sessions;

public class SessionNotFoundException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified session could not be found.";

  public Guid? TenantId
  {
    get => (Guid?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }
  public Guid SessionId
  {
    get => (Guid)Data[nameof(SessionId)]!;
    private set => Data[nameof(SessionId)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public SessionNotFoundException(SessionId id, string? propertyName = null) : base(BuildMessage(id, propertyName))
  {
    TenantId = id.TenantId?.ToGuid();
    SessionId = id.EntityId.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(SessionId id, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), id.TenantId?.ToGuid(), "<null>")
    .AddData(nameof(SessionId), id.EntityId.ToGuid())
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
