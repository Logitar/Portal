using Logitar.Identity.Core;

namespace Logitar.Portal.Application.Roles;

public class RolesNotFoundException : Exception
{
  private const string ErrorMessage = "The specified roles could not be found.";

  public Guid? TenantId
  {
    get => (Guid?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }
  public IReadOnlyCollection<string> Roles
  {
    get => (IReadOnlyCollection<string>)Data[nameof(Roles)]!;
    private set => Data[nameof(Roles)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public RolesNotFoundException(TenantId? tenantId, IEnumerable<string> roles, string? propertyName = null) : base(BuildMessage(tenantId, roles, propertyName))
  {
    Roles = roles.ToArray();
    PropertyName = propertyName;
  }

  private static string BuildMessage(TenantId? tenantId, IEnumerable<string> roles, string? propertyName)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(TenantId)).Append(": ").AppendLine(tenantId?.ToGuid().ToString() ?? "<null>");
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName ?? "<null>");
    message.AppendLine("Roles:");
    foreach (string role in roles)
    {
      message.Append(" - ").AppendLine(role);
    }
    return message.ToString();
  }
}
