using System.Globalization;

namespace Portal.Web.Models.Api.Culture
{
  public class LocaleModel
  {
    public LocaleModel(CultureInfo culture)
    {
      ArgumentNullException.ThrowIfNull(culture);

      Code = culture.Name;
      DisplayName = culture.EnglishName;
    }

    public string Code { get; }
    public string DisplayName { get; }
  }
}
