namespace Logitar.Portal.Core.Users.Contact;

internal record CountrySettings
{
  public string? PostalCode { get; init; }
  public HashSet<string>? Regions { get; init; }
}

