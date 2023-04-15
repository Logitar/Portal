using Logitar.Portal.Core;
using Logitar.Portal.Domain.Realms;
using System.Net;
using System.Text;

namespace Logitar.Portal.Application.Emails.Messages
{
  internal class UsersNotInRealmException : ApiException
  {
    public UsersNotInRealmException(IEnumerable<Guid> ids, Realm? realm, string paramName)
      : base(HttpStatusCode.BadRequest, GetMessage(ids, realm, paramName))
    {
      Ids = ids ?? throw new ArgumentNullException(nameof(ids));
      ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));
      Realm = realm;

      Value = new Dictionary<string, object?>
      {
        ["code"] = nameof(UsersNotInRealmException).Remove(nameof(Exception)),
        [nameof(realm)] = realm?.ToString(),
        [paramName] = ids
      };
    }

    public IEnumerable<Guid> Ids { get; }
    public string? ParamName { get; }
    public Realm? Realm { get; }

    private static string GetMessage(IEnumerable<Guid> ids, Realm? realm, string paramName)
    {
      var message = new StringBuilder();

      message.AppendLine("The specified users do not belong to the specified realm.");
      message.AppendLine($"User IDs: {string.Join(", ", ids)}");
      message.AppendLine($"Realm: {realm}");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
