using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Users;

public class UserNotFoundException : InvalidCredentialsException
{
  private new const string ErrorMessage = "The specified user could not be found.";

  public UserNotFoundException(RealmAggregate? realm, string uniqueName) : base(BuildMessage(realm, uniqueName))
  {
    Realm = realm?.ToString();
    UniqueName = uniqueName;
  }

  public string? Realm
  {
    get => (string?)Data[nameof(Realm)];
    private set => Data[nameof(Realm)] = value;
  }
  public string UniqueName
  {
    get => (string)Data[nameof(UniqueName)]!;
    private set => Data[nameof(UniqueName)] = value;
  }

  private static string BuildMessage(RealmAggregate? realm, string uniqueName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("Realm: ").AppendLine(realm?.ToString() ?? "<null>");
    message.Append("UniqueName: ").AppendLine(uniqueName);

    return message.ToString();
  }
}
