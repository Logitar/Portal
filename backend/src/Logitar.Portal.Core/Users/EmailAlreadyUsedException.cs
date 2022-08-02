using System.Net;
using System.Text;

namespace Logitar.Portal.Core.Users
{
  internal class EmailAlreadyUsedException : ApiException
  {
    public EmailAlreadyUsedException(string email, string paramName)
      : base(HttpStatusCode.Conflict, GetMessage(email, paramName))
    {
      Email = email ?? throw new ArgumentNullException(nameof(email));
      ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));

      Value = new { field = paramName };
    }

    public string Email { get; }
    public string? ParamName { get; }

    private static string GetMessage(string email, string paramName)
    {
      var message = new StringBuilder();

      message.AppendLine($"The email address '{email}' is already used.");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
