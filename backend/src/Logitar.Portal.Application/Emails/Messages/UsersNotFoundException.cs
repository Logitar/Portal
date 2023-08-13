using System.Net;
using System.Text;

namespace Logitar.Portal.Application.Emails.Messages
{
  internal class UsersNotFoundException : ApiException
  {
    public UsersNotFoundException(IEnumerable<string> ids, string paramName)
      : base(HttpStatusCode.NotFound, GetMessage(ids, paramName))
    {
      Ids = ids ?? throw new ArgumentNullException(nameof(ids));
      ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));

      Value = new Dictionary<string, object?>
      {
        [paramName] = ids
      };
    }

    public IEnumerable<string> Ids { get; }
    public string ParamName { get; }

    private static string GetMessage(IEnumerable<string> ids, string paramName)
    {
      var message = new StringBuilder();

      message.AppendLine("The specified users could not be found.");
      message.AppendLine($"Ids: {string.Join(", ", ids)}");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
