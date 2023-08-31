namespace Logitar.Portal.Domain.Users;

internal record CountrySettings
{
  public string? PostalCode { get; init; }
  public HashSet<string>? Regions { get; init; }
}
