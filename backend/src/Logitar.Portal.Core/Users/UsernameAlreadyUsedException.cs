using System.Text;

namespace Logitar.Portal.Core.Users
{
  internal class UsernameAlreadyUsedException : Exception
  {
    public UsernameAlreadyUsedException(string email, string paramName) : base(GetMessage(email, paramName))
    {
      Data["Username"] = email;
      Data["ParamName"] = paramName;
    }

    private static string GetMessage(string email, string paramName)
    {
      StringBuilder message = new();

      message.AppendLine("The specified username is already used.");
      message.AppendLine($"Username: {email}");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
