using System.Text;

namespace Logitar.Portal.Application.Templates
{
  public class KeyAlreadyUsedException : Exception
  {
    public KeyAlreadyUsedException(string key, string paramName)
      : base(GetMessage(key, paramName))
    {
      Data["Key"] = key;
      Data["ParamName"] = paramName;
    }

    private static string GetMessage(string key, string paramName)
    {
      StringBuilder message = new();

      message.AppendLine("The specified key is already used.");
      message.AppendLine($"Key: {key}");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
