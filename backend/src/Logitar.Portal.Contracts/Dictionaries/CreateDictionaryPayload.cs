namespace Logitar.Portal.Contracts.Dictionaries;

public record CreateDictionaryPayload
{
  public string Locale { get; set; }

  public CreateDictionaryPayload() : this(string.Empty)
  {
  }

  public CreateDictionaryPayload(string locale)
  {
    Locale = locale;
  }
}
