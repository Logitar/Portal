using System.Net;
using System.Text;

namespace Logitar.Portal.Application.Realms
{
  internal class AliasAlreadyUsedException : ApiException
  {
    public AliasAlreadyUsedException(string alias, string paramName)
      : base(HttpStatusCode.Conflict, GetMessage(alias, paramName))
    {
      Alias = alias ?? throw new ArgumentNullException(nameof(alias));
      ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));

      Value = new { field = paramName };
    }

    public string Alias { get; }
    public string? ParamName { get; }

    private static string GetMessage(string alias, string paramName)
    {
      var message = new StringBuilder();

      message.AppendLine($"The alias '{alias}' is already used.");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
