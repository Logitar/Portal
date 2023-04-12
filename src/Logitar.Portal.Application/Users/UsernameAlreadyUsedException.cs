using System.Net;
using System.Text;

namespace Logitar.Portal.Application.Users
{
  internal class UsernameAlreadyUsedException : ApiException
  {
    public UsernameAlreadyUsedException(string username, string paramName)
      : base(HttpStatusCode.Conflict, GetMessage(username, paramName))
    {
      ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));
      Username = username ?? throw new ArgumentNullException(nameof(username));

      Value = new { field = paramName };
    }

    public string? ParamName { get; }
    public string Username { get; }

    private static string GetMessage(string alias, string paramName)
    {
      var message = new StringBuilder();

      message.AppendLine($"The username '{alias}' is already used.");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
