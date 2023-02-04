using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Globalization;

namespace Logitar.Portal.Infrastructure.ValueConverters
{
  internal class CultureInfoValueConverter : ValueConverter<CultureInfo?, string?>
  {
    public CultureInfoValueConverter()
      : base(culture => culture == null ? null : culture.Name, name => name == null ? null : CultureInfo.GetCultureInfo(name))
    {
    }
  }
}
