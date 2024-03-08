using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Constants;

namespace Logitar.Portal.Application.Users;

public class ImpersonifiedUserNotFoundException : Exception
{
  public const string ErrorMessage = "The specified user could not be found.";

  public TenantId? TenantId
  {
    get => TenantId.TryCreate((string?)Data[nameof(TenantId)]);
    private set => Data[nameof(TenantId)] = value?.Value;
  }
  public string User
  {
    get => (string)Data[nameof(User)]!;
    private set => Data[nameof(User)] = value;
  }
  public string Header
  {
    get => (string)Data[nameof(Header)]!;
    private set => Data[nameof(Header)] = value;
  }

  public ImpersonifiedUserNotFoundException(TenantId? tenantId, string user) : base(BuildMessage(tenantId, user))
  {
    TenantId = tenantId;
    User = user;
    Header = Headers.User;
  }

  private static string BuildMessage(TenantId? tenantId, string user) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), tenantId?.Value, "<null>")
    .AddData(nameof(User), user)
    .AddData(nameof(Header), Headers.User)
    .Build();
}
