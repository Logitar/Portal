using System.Text;

namespace Logitar.Portal.Core.Users;

public class UsersNotFoundException : Exception
{
  public UsersNotFoundException(IEnumerable<string> ids, string paramName) : base(GetMessage(ids, paramName))
  {
    Data[nameof(Users)] = ids;
    Data[nameof(ParamName)] = paramName;
  }

  public IEnumerable<string> Users => (IEnumerable<string>)Data[nameof(Users)]!;
  public string ParamName => (string)Data[nameof(ParamName)]!;

  private static string GetMessage(IEnumerable<string> ids, string paramName)
  {
    StringBuilder message = new();

    message.AppendLine("The specified users could not be found.");
    message.Append("ParamName: ").AppendLine(paramName);

    message.AppendLine();
    message.AppendLine("Users:");
    foreach (string id in ids)
    {
      message.Append(" - ").AppendLine(id);
    }

    return message.ToString();
  }
}
