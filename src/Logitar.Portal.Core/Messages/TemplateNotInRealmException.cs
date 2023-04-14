using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Templates;
using System.Text;

namespace Logitar.Portal.Core.Messages;

public class TemplateNotInRealmException : Exception
{
  public TemplateNotInRealmException(TemplateAggregate template, RealmAggregate? realm, string paramName)
    : base(GetMessage(template, realm, paramName))
  {
    Data["Template"] = template.ToString();
    Data["Realm"] = realm?.ToString() ?? "Portal";
    Data["ParamName"] = paramName;
  }

  private static string GetMessage(TemplateAggregate template, RealmAggregate? realm, string paramName)
  {
    StringBuilder message = new();

    message.AppendLine("The specified template does not belong to the specified realm.");
    message.Append("Template: ").Append(template).AppendLine();
    message.Append("Realm: ").Append(realm?.ToString() ?? "Portal").AppendLine();
    message.Append("ParamName: ").AppendLine(paramName);

    return message.ToString();
  }
}
