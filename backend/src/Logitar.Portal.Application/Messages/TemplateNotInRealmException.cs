using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Templates;
using System.Text;

namespace Logitar.Portal.Application.Messages
{
  public class TemplateNotInRealmException : Exception
  {
    public TemplateNotInRealmException(Template template, Realm? realm, string paramName)
      : base(GetMessage(template, realm, paramName))
    {
      Data["Template"] = template.ToString();
      Data["Realm"] = realm?.ToString() ?? "Portal";
      Data["ParamName"] = paramName;
    }

    private static string GetMessage(Template template, Realm? realm, string paramName)
    {
      StringBuilder message = new();

      message.AppendLine("The specified template does not belong to the specified realm.");
      message.AppendLine($"Template: {template}");
      message.AppendLine($"Realm: {(realm?.ToString() ?? "Portal")}");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
