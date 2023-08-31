using FluentValidation.Results;

namespace Logitar.Portal.Application.Roles;

public class RolesNotFoundException : Exception
{
  private const string ErrorMessage = "The specified roles could not be found.";

  public RolesNotFoundException(IEnumerable<string> missingRoles, string propertyName)
    : base(BuildMessage(missingRoles, propertyName))
  {
    MissingRoles = missingRoles;
    PropertyName = propertyName;
  }

  public IEnumerable<string> MissingRoles
  {
    get => (IEnumerable<string>)Data[nameof(MissingRoles)]!;
    private set => Data[nameof(MissingRoles)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, attemptedValue: string.Join(", ", MissingRoles))
  {
    ErrorCode = "RolesNotFound"
  };

  private static string BuildMessage(IEnumerable<string> missingRoles, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("PropertyName: ").AppendLine(propertyName);

    message.AppendLine().Append("MissingRoles:");
    foreach (string missingRole in missingRoles)
    {
      message.Append(" - ").AppendLine(missingRole);
    }

    return message.ToString();
  }
}
