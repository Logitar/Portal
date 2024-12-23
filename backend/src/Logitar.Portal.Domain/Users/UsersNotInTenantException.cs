using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;

namespace Logitar.Portal.Domain.Users;

public class UsersNotInTenantException : Exception
{
  public const string ErrorMessage = "The specified users are not in the specified tenant.";

  public IReadOnlyCollection<string> UserIds
  {
    get => (IReadOnlyCollection<string>)Data[nameof(UserIds)]!;
    private set => Data[nameof(UserIds)] = value;
  }
  public string? ExpectedTenantId
  {
    get => (string?)Data[nameof(ExpectedTenantId)];
    private set => Data[nameof(ExpectedTenantId)] = value;
  }

  public UsersNotInTenantException(IEnumerable<UserId> userIds, TenantId? expectedTenant) : base(BuildMessage(userIds, expectedTenant))
  {
    UserIds = userIds.Select(id => id.Value).ToArray();
    ExpectedTenantId = expectedTenant?.Value;
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
