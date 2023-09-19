namespace Logitar.Portal.Domain.Users;

internal static class PostalAddressHelper
{
  private static readonly Dictionary<string, CountrySettings> _countries = new();

  static PostalAddressHelper()
  {
    _countries["CA"] = new CountrySettings
    {
      PostalCode = "[ABCEGHJ-NPRSTVXY]\\d[ABCEGHJ-NPRSTV-Z][ -]?\\d[ABCEGHJ-NPRSTV-Z]\\d$",
      Regions = new HashSet<string>(new[] { "AB", "BC", "MB", "NB", "NL", "NT", "NS", "NU", "ON", "PE", "QC", "SK", "YT" })
    };
  }

  public static IEnumerable<string> SupportedCountries => _countries.Keys;
  public static bool IsSupported(string country) => _countries.ContainsKey(country);

  public static CountrySettings? GetCountry(string country)
    => _countries.TryGetValue(country, out CountrySettings? settings) ? settings : null;
}
