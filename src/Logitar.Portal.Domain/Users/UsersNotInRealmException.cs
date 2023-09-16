using FluentValidation.Results;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Domain.Users;

public class UsersNotInRealmException : Exception, IFailureException
{
  private const string ErrorMessage = "The specified users must be in the specified realm.";

  public UsersNotInRealmException(IEnumerable<UserAggregate> users, RealmAggregate? realm, string propertyName)
    : base(BuildMessage(users, realm, propertyName))
  {
    Users = users.Select(user => user.ToString());
    Realm = realm?.ToString();
    PropertyName = propertyName;
  }

  public IEnumerable<string> Users
  {
    get => (IEnumerable<string>)Data[nameof(Users)]!;
    private set => Data[nameof(Users)] = value;
  }
  public string? Realm
  {
    get => (string?)Data[nameof(Realm)];
    private set => Data[nameof(Realm)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, Users)
  {
    ErrorCode = "UsersNotInRealm",
    CustomState = new { Realm }
  };

  private static string BuildMessage(IEnumerable<UserAggregate> users, RealmAggregate? realm, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("Realm: ").AppendLine(realm?.ToString() ?? "<null>");
    message.Append("PropertyName: ").AppendLine(propertyName);

    message.AppendLine("Users:");
    foreach (UserAggregate user in users)
    {
      message.Append(" - ").Append(user).AppendLine();
    }

    return message.ToString();
  }
}
