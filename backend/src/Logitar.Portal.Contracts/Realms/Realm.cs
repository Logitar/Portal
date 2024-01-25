namespace Logitar.Portal.Contracts.Realms;

public class Realm : Aggregate
{
  public string UniqueSlug { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string? DefaultLocale { get; set; }
  public string Secret { get; set; }
  public string? Url { get; set; }

  public UniqueNameSettings UniqueNameSettings { get; set; } = new();
  public PasswordSettings PasswordSettings { get; set; } = new();
  public bool RequireUniqueEmail { get; set; } = true;

  public List<CustomAttribute> CustomAttributes { get; set; }

  public Realm() : this(string.Empty, string.Empty)
  {
  }

  public Realm(string uniqueSlug, string secret)
  {
    UniqueSlug = uniqueSlug;
    Secret = secret;
    CustomAttributes = [];
  }
}
