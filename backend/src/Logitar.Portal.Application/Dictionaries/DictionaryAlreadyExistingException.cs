using Logitar.Portal.Core;
using System.Net;
using System.Text;

namespace Logitar.Portal.Application.Dictionaries
{
  internal class DictionaryAlreadyExistingException : ApiException
  {
    public DictionaryAlreadyExistingException(Guid? realmId, string locale)
      : base(HttpStatusCode.Conflict, GetMessage(realmId, locale))
    {
      Locale = locale ?? throw new ArgumentNullException(nameof(locale));
      RealmId = realmId;
      Value = new { code = nameof(DictionaryAlreadyExistingException).Remove(nameof(Exception)) };
    }

    public string Locale { get; }
    public Guid? RealmId { get; }

    private static string GetMessage(Guid? realmId, string locale)
    {
      var message = new StringBuilder();

      message.AppendLine("The dictionary already exists.");
      message.AppendLine($"Realm: {(realmId?.ToString() ?? "Portal (no realm)")}");
      message.AppendLine($"Locale: {locale}");

      return message.ToString();
    }
  }
}
