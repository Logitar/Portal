using System.Net;
using System.Text;

namespace Portal.Core.Emails.Templates
{
  internal class KeyAlreadyUsedException : ApiException
  {
    public KeyAlreadyUsedException(string key, string paramName)
      : base(HttpStatusCode.Conflict, GetMessage(key, paramName))
    {
      Key = key ?? throw new ArgumentNullException(nameof(key));
      ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));

      Value = new { field = paramName };
    }

    public string Key { get; }
    public string? ParamName { get; }

    private static string GetMessage(string key, string paramName)
    {
      var message = new StringBuilder();

      message.AppendLine($"The key '{key}' is already used.");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
