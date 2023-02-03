using Logitar.Portal.Domain.Realms;
using System.Globalization;

namespace Logitar.Portal.Application.Dictionaries
{
  public class DictionaryAlreadyExistingException : Exception
  {
    public DictionaryAlreadyExistingException(Realm? realm, CultureInfo locale) : base()
    {
      Data["Realm"] = realm?.ToString() ?? nameof(Portal);
      Data["Locale"] = locale.ToString();
    }
  }
}
