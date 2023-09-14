using FluentValidation.Results;

namespace Logitar.Portal.Application.Users;

public class UsersNotFoundException : Exception
{
  private const string ErrorMessage = "The specified users could not be found.";

  public UsersNotFoundException(IEnumerable<string> missingUsers, string propertyName)
    : base(BuildMessage(missingUsers, propertyName))
  {
    MissingUsers = missingUsers;
    PropertyName = propertyName;
  }

  public IEnumerable<string> MissingUsers
  {
    get => (IEnumerable<string>)Data[nameof(MissingUsers)]!;
    private set => Data[nameof(MissingUsers)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, attemptedValue: string.Join(", ", MissingUsers))
  {
    ErrorCode = "UsersNotFound"
  };

  private static string BuildMessage(IEnumerable<string> missingUsers, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("PropertyName: ").AppendLine(propertyName);

    message.AppendLine().Append("MissingUsers:");
    foreach (string missingUser in missingUsers)
    {
      message.Append(" - ").AppendLine(missingUser);
    }

    return message.ToString();
  }
}
