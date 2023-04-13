using System.Globalization;
using System.Text;

namespace Logitar.Portal.Core.Dictionaries;

public class LocaleAlreadyUsedException : Exception, IPropertyFailure
{
  public LocaleAlreadyUsedException(CultureInfo locale, string paramName)
    : base(GetMessage(locale, paramName))
  {
    Data[nameof(Value)] = locale.ToString();
    Data[nameof(ParamName)] = paramName;
  }

  public string ParamName => (string)Data[nameof(ParamName)]!;
  public string Value => (string)Data[nameof(Value)]!;

  private static string GetMessage(CultureInfo locale, string paramName)
  {
    StringBuilder message = new();

    message.Append("The locale '").Append(locale).AppendLine("' is already used.");
    message.Append("ParamName: ").Append(paramName).AppendLine();

    return message.ToString();
  }
}
