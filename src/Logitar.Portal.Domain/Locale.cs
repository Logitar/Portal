namespace Logitar.Portal.Domain;

public record Locale
{
  private const string DefaultLocaleCode = "en";
  private const int LOCALE_CUSTOM_UNSPECIFIED = 0x1000;

  private readonly CultureInfo _culture;

  public Locale(string code)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("The locale code is required.", nameof(code));
    }

    _culture = CultureInfo.GetCultureInfo(code.Trim());
    if (_culture.LCID == LOCALE_CUSTOM_UNSPECIFIED)
    {
      throw new ArgumentOutOfRangeException(nameof(code), "The locale cannot be an user-defined culture.");
    }

    if (!string.IsNullOrWhiteSpace(_culture.Parent?.Name))
    {
      Parent = new Locale(_culture.Parent.Name);
    }
  }

  public static Locale Default { get; } = new(DefaultLocaleCode);

  public string Code => _culture.Name;
  public Locale? Parent { get; }
}
