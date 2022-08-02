using System.Globalization;

namespace Logitar.Portal.Web.Models.Api.Culture
{
  public class LocaleSummary
  {
    public LocaleSummary(CultureInfo culture)
    {
      ArgumentNullException.ThrowIfNull(culture);

      Code = culture.Name;
      DisplayName = culture.EnglishName;
    }

    public string Code { get; }
    public string DisplayName { get; }
  }
}
