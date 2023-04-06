using System.Text;

namespace Logitar.Portal.v2.Core;

internal class InvalidLocaleException : Exception
{
  public InvalidLocaleException(string value, string paramName, Exception innerException)
    : base(GetMessage(value, paramName), innerException)
  {
    Data["Value"] = value;
    Data["ParamName"] = paramName;
  }

  private static string GetMessage(string value, string paramName)
  {
    StringBuilder message = new();

    message.Append("The locale '").Append(value).AppendLine("' is not valid.");
    message.Append("ParamName: ").Append(paramName).AppendLine();

    return message.ToString();
  }
}
