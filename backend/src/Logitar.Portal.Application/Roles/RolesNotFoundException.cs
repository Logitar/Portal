using System.Text;

namespace Logitar.Portal.Application.Roles;

public class RolesNotFoundException : Exception
{
  public const string ErrorMessage = "The specified roles could not be found.";

  public IEnumerable<string> Roles
  {
    get => (IEnumerable<string>)Data[nameof(Roles)]!;
    private set => Data[nameof(Roles)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public RolesNotFoundException(IEnumerable<string> roles, string? propertyName = null) : base(BuildMessage(roles, propertyName))
  {
    Roles = roles;
    PropertyName = propertyName;
  }

  private static string BuildMessage(IEnumerable<string> roles, string? propertyName)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName ?? "<null>");
    message.AppendLine("Roles:");
    foreach (string role in roles)
    {
      message.Append(" - ").AppendLine(role);
    }
    return message.ToString();
  }
}
