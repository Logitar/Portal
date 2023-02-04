using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using System.Text;

namespace Logitar.Portal.Application.Messages
{
  public class SenderNotInRealmException : Exception
  {
    public SenderNotInRealmException(Sender sender, Realm? realm, string paramName)
      : base(GetMessage(sender, realm, paramName))
    {
      Data["Sender"] = sender.ToString();
      Data["Realm"] = realm?.ToString() ?? "Portal";
      Data["ParamName"] = paramName;
    }

    private static string GetMessage(Sender sender, Realm? realm, string paramName)
    {
      StringBuilder message = new();

      message.AppendLine("The specified sender does not belong to the specified realm.");
      message.AppendLine($"Sender: {sender}");
      message.AppendLine($"Realm: {(realm?.ToString() ?? "Portal")}");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
