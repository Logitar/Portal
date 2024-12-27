using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;

namespace Logitar.Portal.Domain.Users;

public class UsersNotInTenantException : Exception
{
  private const string ErrorMessage = "The specified users are not in the specified tenant.";

  public IReadOnlyCollection<Guid> UserIds
  {
    get => (IReadOnlyCollection<Guid>)Data[nameof(UserIds)]!;
    private set => Data[nameof(UserIds)] = value;
  }
  public Guid? ExpectedTenantId
  {
    get => (Guid?)Data[nameof(ExpectedTenantId)];
    private set => Data[nameof(ExpectedTenantId)] = value;
  }

  public UsersNotInTenantException(IEnumerable<UserId> userIds, TenantId? expectedTenant) : base(BuildMessage(userIds, expectedTenant))
  {
    UserIds = userIds.Select(id => id.EntityId.ToGuid()).ToArray();
    ExpectedTenantId = expectedTenant?.ToGuid();
  }

  private static string BuildMessage(IEnumerable<UserId> userIds, TenantId? expectedTenant)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.AppendLine(nameof(ExpectedTenantId)).Append(": ").AppendLine(expectedTenant?.ToGuid().ToString() ?? "<null>");
    message.Append(nameof(UserIds)).AppendLine(":");
    foreach (UserId userId in userIds)
    {
      message.AppendLine(" - ").AppendLine(userId.Value);
    }
    return message.ToString();
  }
}
