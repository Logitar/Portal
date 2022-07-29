using Portal.Core.Emails.Templates;
using Portal.Core.Realms;
using System.Net;
using System.Text;

namespace Portal.Core.Emails.Messages
{
  internal class TemplateNotInRealmException : ApiException
  {
    public TemplateNotInRealmException(Template template, Realm? realm, string paramName)
      : base(HttpStatusCode.BadRequest, GetMessage(template, realm, paramName))
    {
      ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));
      Realm = realm;
      Template = template ?? throw new ArgumentNullException(nameof(template));

      Value = new Dictionary<string, object?>
      {
        ["code"] = nameof(TemplateNotInRealmException).Remove(nameof(Exception)),
        ["realm"] = realm?.ToString(),
        [paramName] = template.ToString()
      };
    }
    
    public string? ParamName { get; }
    public Realm? Realm { get; }
    public Template Template { get; }

    private static string GetMessage(Template template, Realm? realm, string paramName)
    {
      var message = new StringBuilder();

      message.AppendLine("The specified template does not belong to the specified realm.");
      message.AppendLine($"Template: {template}");
      message.AppendLine($"Realm: {realm}");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
