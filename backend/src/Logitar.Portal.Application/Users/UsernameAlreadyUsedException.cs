using System.Text;

namespace Logitar.Portal.Application.Users
{
  public class UsernameAlreadyUsedException : Exception
  {
    public UsernameAlreadyUsedException(string username, string paramName)
      : base(GetMessage(username, paramName))
    {
      Data["Username"] = username;
      Data["ParamName"] = paramName;
    }

    private static string GetMessage(string username, string paramName)
    {
      StringBuilder message = new();

      message.AppendLine("The specified username is already used.");
      message.AppendLine($"Username: {username}");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
