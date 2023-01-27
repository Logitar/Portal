using System.Text;

namespace Logitar.Portal.Core.Realms
{
  internal class AliasAlreadyUsedException : Exception
  {
    public AliasAlreadyUsedException(string alias, string paramName) : base(GetMessage(alias, paramName))
    {
      Data["Alias"] = alias;
      Data["ParamName"] = paramName;
    }

    private static string GetMessage(string alias, string paramName)
    {
      StringBuilder message = new();

      message.AppendLine("The specified alias is already used.");
      message.AppendLine($"Alias: {alias}");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
