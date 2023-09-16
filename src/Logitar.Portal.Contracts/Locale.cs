namespace Logitar.Portal.Contracts;

public record Locale
{
  public int Id { get; set; }
  public string Code { get; set; } = string.Empty;
  public string DisplayName { get; set; } = string.Empty;
  public string EnglishName { get; set; } = string.Empty;
  public string NativeName { get; set; } = string.Empty;
}
