using Logitar.Portal.Core;
using System.Net;
using System.Text;

namespace Logitar.Portal.Application.Emails.Messages
{
  internal class UsersEmailRequiredException : ApiException
  {
    public UsersEmailRequiredException(IEnumerable<Guid> ids, string paramName)
      : base(HttpStatusCode.BadRequest, GetMessage(ids, paramName))
    {
      Ids = ids ?? throw new ArgumentNullException(nameof(ids));
      ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));

      Value = new Dictionary<string, object?>
      {
        ["code"] = nameof(UsersEmailRequiredException).Remove(nameof(Exception)),
        [paramName] = ids
      };
    }

    public IEnumerable<Guid> Ids { get; }
    public string? ParamName { get; }

    private static string GetMessage(IEnumerable<Guid> ids, string paramName)
    {
      var message = new StringBuilder();

      message.AppendLine("The specified users do not have an email address.");
      message.AppendLine($"User IDs: {string.Join(", ", ids)}");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
