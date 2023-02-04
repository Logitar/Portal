using Logitar.Portal.Domain.Users;
using System.Text;

namespace Logitar.Portal.Application.Messages
{
  public class UsersEmailRequiredException : Exception
  {
    public UsersEmailRequiredException(IEnumerable<User> users, string paramName)
      : base(GetMessage(users, paramName))
    {
      Data["Ids"] = users.Select(u => u.Id);
      Data["ParamName"] = paramName;
    }

    private static string GetMessage(IEnumerable<User> users, string paramName)
    {
      StringBuilder message = new();

      message.AppendLine("The specified users do not have an email address.");
      message.AppendLine($"ParamName: {paramName}");
      message.AppendLine("Users:");
      foreach (User user in users)
      {
        message.AppendLine($"- {user}");
      }

      return message.ToString();
    }
  }
}
