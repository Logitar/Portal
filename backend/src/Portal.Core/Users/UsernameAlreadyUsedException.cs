using System.Net;
using System.Text;

namespace Portal.Core.Users
{
  internal class UsernameAlreadyUsedException : ApiException
  {
    public UsernameAlreadyUsedException(string username, string paramName)
      : base(HttpStatusCode.Conflict, GetMessage(username, paramName))
    {
      Username = username ?? throw new ArgumentNullException(nameof(username));
      ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));

      Value = new { field = paramName };
    }

    public string? ParamName { get; }
    public string Username { get; }

    private static string GetMessage(string alias, string paramName)
    {
      var message = new StringBuilder();

      message.AppendLine($"The alias '{alias}' is already used.");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
