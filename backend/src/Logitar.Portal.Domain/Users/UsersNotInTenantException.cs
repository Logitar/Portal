using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;

namespace Logitar.Portal.Domain.Users;

public class UsersNotInTenantException : Exception
{
  public const string ErrorMessage = "The specified users are not in the specified tenant.";

  public IEnumerable<UserId> UserIds
  {
    get => ((IEnumerable<string>)Data[nameof(UserIds)]!).Select(value => new UserId(value));
    private set => Data[nameof(UserIds)] = value.Select(id => id.Value);
  }
  public TenantId? ExpectedTenantId
  {
    get => TenantId.TryCreate((string?)Data[nameof(ExpectedTenantId)]);
    private set => Data[nameof(ExpectedTenantId)] = value?.Value;
  }

  public UsersNotInTenantException(IEnumerable<UserId> userIds, TenantId? expectedTenant) : base(BuildMessage(userIds, expectedTenant))
  {
    UserIds = userIds;
    ExpectedTenantId = expectedTenant;
  }

  private static string BuildMessage(IEnumerable<UserId> userIds, TenantId? expectedTenant)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.AppendLine(nameof(ExpectedTenantId)).Append(": ").AppendLine(expectedTenant?.Value ?? "<null>");
    message.Append(nameof(UserIds)).AppendLine(":");
    foreach (UserId userId in userIds)
    {
      message.AppendLine(" - ").AppendLine(userId.Value);
    }
    return message.ToString();
  }
}
