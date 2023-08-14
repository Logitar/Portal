namespace Logitar.Portal.Application;

internal static class InputExtensions
{
  public static CultureInfo? GetCultureInfo(this string name, string propertyName)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      return null;
    }

    try
    {
      return CultureInfo.GetCultureInfo(name.Trim());
    }
    catch (Exception innerException)
    {
      throw new InvalidLocaleException(name, propertyName, innerException);
    }
  }
}
