using Logitar.Identity.Core;

namespace Logitar.Portal.Application.Messages;

public class MissingRecipientContactsException : Exception
{
  private const string ErrorMessage = "The specified recipients are missing an email address.";

  public Guid? TenantId
  {
    get => (Guid?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }
  public IReadOnlyCollection<Guid> UserIds
  {
    get => (IReadOnlyCollection<Guid>)Data[nameof(UserIds)]!;
    private set => Data[nameof(UserIds)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public MissingRecipientContactsException(TenantId? tenantId, IEnumerable<Guid> userIds, string propertyName) : base(BuildMessage(tenantId, userIds, propertyName))
  {
    TenantId = tenantId?.ToGuid();
    UserIds = userIds.ToArray();
    PropertyName = propertyName;
  }

  private static string BuildMessage(TenantId? tenantId, IEnumerable<Guid> userIds, string propertyName)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(TenantId)).Append(": ").AppendLine(tenantId?.ToGuid().ToString() ?? "<null>");
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName ?? "<null>");
    message.Append(nameof(UserIds)).AppendLine(":");
    foreach (Guid userId in userIds)
    {
      message.Append(" - ").Append(userId).AppendLine();
    }
    return message.ToString();
  }
}
