using System.Text;

namespace Logitar.Portal.v2.Core.Realms;

internal class UniqueNameAlreadyUsedException : Exception
{
  public UniqueNameAlreadyUsedException(string uniqueName, string paramName)
    : base(GetMessage(uniqueName, paramName))
  {
    Data["UniqueName"] = uniqueName;
    Data["ParamName"] = paramName;
  }

  private static string GetMessage(string uniqueName, string paramName)
  {
    StringBuilder message = new();

    message.Append("The specified name '").Append(uniqueName).AppendLine("' is already used.");
    message.Append("ParamName: ").Append(paramName).AppendLine();

    return message.ToString();
  }
}
