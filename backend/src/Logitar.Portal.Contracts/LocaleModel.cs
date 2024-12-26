namespace Logitar.Portal.Contracts;

public class LocaleModel
{
  public int Id { get; set; }
  public string Code { get; set; }
  public string DisplayName { get; set; }
  public string EnglishName { get; set; }
  public string NativeName { get; set; }

  public LocaleModel() : this(string.Empty)
  {
  }

  public LocaleModel(string cultureName) : this(CultureInfo.GetCultureInfo(cultureName.Trim()))
  {
  }

  public LocaleModel(CultureInfo culture) : this(culture.LCID, culture.Name, culture.DisplayName, culture.EnglishName, culture.NativeName)
  {
  }

  public LocaleModel(int id, string code, string displayName, string englishName, string nativeName)
  {
    Id = id;
    Code = code;
    DisplayName = displayName;
    EnglishName = englishName;
    NativeName = nativeName;
  }

  public static LocaleModel? TryCreate(string? code) => string.IsNullOrWhiteSpace(code) ? null : new(code);

  public override bool Equals(object? obj) => obj is LocaleModel locale && locale.Id == Id;
  public override int GetHashCode() => HashCode.Combine(GetType(), Id);
  public override string ToString() => DisplayName;
}
