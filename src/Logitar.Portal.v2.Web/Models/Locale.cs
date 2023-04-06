using System.Globalization;

namespace Logitar.Portal.v2.Web.Models;

public record Locale
{
  public Locale(CultureInfo culture)
  {
    Code = culture.Name;
    DisplayName = culture.EnglishName;
  }

  public string Code { get; }
  public string DisplayName { get; }
}
