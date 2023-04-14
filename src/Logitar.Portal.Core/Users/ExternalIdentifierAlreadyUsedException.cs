using Logitar.Portal.Core.Realms;
using System.Text;

namespace Logitar.Portal.Core.Users;

public class ExternalIdentifierAlreadyUsedException : Exception
{
  public ExternalIdentifierAlreadyUsedException(RealmAggregate? realm, string key, string value)
    : base(GetMessage(realm, key, value))
  {
    Data["Realm"] = realm?.ToString() ?? "Portal";
    Data["Key"] = key;
    Data["Value"] = value;
  }

  private static string GetMessage(RealmAggregate? realm, string key, string value)
  {
    StringBuilder message = new();

    message.AppendLine("The specified external identifier is already used.");
    message.Append("Realm: ").Append(realm?.ToString() ?? "Portal").AppendLine();
    message.Append("Key: ").Append(key).AppendLine();
    message.Append("Value: ").Append(value).AppendLine();

    return message.ToString();
  }
}
