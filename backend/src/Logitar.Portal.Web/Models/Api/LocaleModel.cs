using System.Globalization;

namespace Logitar.Portal.Web.Models.Api
{
  public class LocaleModel
  {
    public LocaleModel(CultureInfo culture)
    {
      Code = culture.Name;
      DisplayName = culture.EnglishName;
    }

    public string Code { get; }
    public string DisplayName { get; }
  }
}
