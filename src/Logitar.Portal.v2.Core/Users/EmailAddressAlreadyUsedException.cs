using Logitar.Portal.v2.Core.Users.Contact;
using System.Text;

namespace Logitar.Portal.v2.Core.Users;

public class EmailAddressAlreadyUsedException : Exception, IPropertyFailure
{
  public EmailAddressAlreadyUsedException(ReadOnlyEmail email, string paramName)
    : base(GetMessage(email, paramName))
  {
    Data[nameof(Value)] = email.Address;
    Data[nameof(ParamName)] = paramName;
  }

  public string ParamName => (string)Data[nameof(ParamName)]!;
  public string Value => (string)Data[nameof(Value)]!;

  private static string GetMessage(ReadOnlyEmail email, string paramName)
  {
    StringBuilder message = new();

    message.Append("The specified email address '").Append(email.Address).AppendLine("' is already used.");
    message.Append("ParamName: ").Append(paramName).AppendLine();

    return message.ToString();
  }
}
