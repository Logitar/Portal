using Portal.Core;
using System.Net;
using System.Text;

namespace Portal.Infrastructure.Users
{
  internal class InvalidPasswordException : ApiException
  {
    public InvalidPasswordException(string passwordString, IEnumerable<Error> errors)
      : base(HttpStatusCode.BadRequest, GetMessage(passwordString, errors))
    {
      PasswordString = passwordString ?? throw new ArgumentNullException(nameof(passwordString));
      Errors = errors ?? throw new ArgumentNullException(nameof(errors));

      Value = new
      {
        code = nameof(InvalidPasswordException).Remove(nameof(Exception)),
        errors
      };
    }

    public IEnumerable<Error> Errors { get; }
    public string PasswordString { get; }

    private static string GetMessage(string passwordString, IEnumerable<Error> errors)
    {
      var message = new StringBuilder();

      message.AppendLine($"The password '{passwordString}' is not valid.");

      foreach (Error error in errors)
      {
        message.AppendLine(error.ToString());
      }

      return message.ToString();
    }
  }
}
