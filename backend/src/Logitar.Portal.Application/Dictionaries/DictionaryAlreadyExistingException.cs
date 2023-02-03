using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Dictionaries
{
  public class DictionaryAlreadyExistingException : Exception
  {
    public DictionaryAlreadyExistingException(Realm? realm, string locale) : base()
    {
      Data["Realm"] = realm?.ToString() ?? nameof(Portal);
      Data["Locale"] = locale;
    }
  }
}
