using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using System.Text;

namespace Logitar.Portal.Application.Messages
{
  public class UsersNotInRealmException : Exception
  {
    public UsersNotInRealmException(IEnumerable<User> users, Realm? realm, string paramName)
      : base(GetMessage(users, realm, paramName))
    {
      Data["Ids"] = users.Select(u => u.Id);
      Data["Realm"] = realm?.ToString() ?? "Portal";
      Data["ParamName"] = paramName;
    }

    private static string GetMessage(IEnumerable<User> users, Realm? realm, string paramName)
    {
      StringBuilder message = new();

      message.AppendLine("The specified users do not belong to the specified realm.");
      message.AppendLine($"Realm: {realm?.ToString() ?? "Portal"}");
      message.AppendLine($"ParamName: {paramName}");
      message.AppendLine("Users:");
      foreach (User user in users)
      {
        message.AppendLine($"- {user}");
      }

      return message.ToString();
    }
  }
}
