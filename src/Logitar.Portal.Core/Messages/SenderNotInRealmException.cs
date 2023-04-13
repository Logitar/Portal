using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Senders;
using System.Text;

namespace Logitar.Portal.Core.Messages;

public class SenderNotInRealmException : Exception
{
  public SenderNotInRealmException(SenderAggregate sender, RealmAggregate realm, string paramName)
    : base(GetMessage(sender, realm, paramName))
  {
    Data["Sender"] = sender.ToString();
    Data["Realm"] = realm.ToString();
    Data["ParamName"] = paramName;
  }

  private static string GetMessage(SenderAggregate sender, RealmAggregate realm, string paramName)
  {
    StringBuilder message = new();

    message.AppendLine("The specified sender does not belong to the specified realm.");
    message.Append("Sender: ").Append(sender).AppendLine();
    message.Append("Realm: ").Append(realm).AppendLine();
    message.Append("ParamName: ").AppendLine(paramName);

    return message.ToString();
  }
}
