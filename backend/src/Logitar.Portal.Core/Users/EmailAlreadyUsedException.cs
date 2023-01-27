using System.Text;

namespace Logitar.Portal.Core.Users
{
  internal class EmailAlreadyUsedException : Exception
  {
    public EmailAlreadyUsedException(string email, string paramName) : base(GetMessage(email, paramName))
    {
      Data["Email"] = email;
      Data["ParamName"] = paramName;
    }

    private static string GetMessage(string email, string paramName)
    {
      StringBuilder message = new();

      message.AppendLine("The specified email is already used.");
      message.AppendLine($"Email: {email}");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
