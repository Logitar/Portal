using Portal.Core.Emails.Senders;
using System.Net;
using System.Text;

namespace Portal.Core.Realms
{
  internal class SenderNotInRealmException : ApiException
  {
    public SenderNotInRealmException(Sender sender, Realm? realm, string paramName)
      : base(HttpStatusCode.BadRequest, GetMessage(sender, realm, paramName))
    {
      ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));
      Realm = realm;
      Sender = sender ?? throw new ArgumentNullException(nameof(sender));

      Value = new Dictionary<string, object?>
      {
        ["code"] = nameof(SenderNotInRealmException).Remove(nameof(Exception)),
        ["realm"] = realm?.ToString(),
        [paramName] = sender.ToString()
      };
    }
    
    public string? ParamName { get; }
    public Realm? Realm { get; }
    public Sender Sender { get; }

    private static string GetMessage(Sender sender, Realm? realm, string paramName)
    {
      var message = new StringBuilder();

      message.AppendLine("The specified sender does not belong to the specified realm.");
      message.AppendLine($"Sender: {sender}");
      message.AppendLine($"Realm: {realm}");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
