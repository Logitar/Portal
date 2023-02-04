using System.Text;

namespace Logitar.Portal.Application.Messages
{
  public class UsersNotFoundException : Exception
  {
    public UsersNotFoundException(IEnumerable<string> ids, string paramName)
      : base(GetMessage(ids, paramName))
    {
      Data["Ids"] = ids;
      Data["ParamName"] = paramName;
    }

    private static string GetMessage(IEnumerable<string> ids, string paramName)
    {
      StringBuilder message = new();

      message.AppendLine("The specified users could not be found.");
      message.AppendLine($"ParamName: {paramName}");
      message.AppendLine("Ids:");
      foreach (string id in ids)
      {
        message.AppendLine($"- {id}");
      }

      return message.ToString();
    }
  }
}
