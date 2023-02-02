using Logitar.Portal.Domain.Users;
using System.Globalization;

namespace Logitar.Portal.Domain.Realms
{
  public class Realm : AggregateRoot
  {
    private Realm() : base()
    {
    }

    public string Alias { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }
    public string? Description { get; private set; }

    public CultureInfo? DefaultLocale { get; private set; }
    public string JwtSecret { get; private set; } = string.Empty;
    public string? Url { get; private set; }

    public bool RequireConfirmedAccount { get; private set; }
    public bool RequireUniqueEmail { get; private set; }

    public UsernameSettings UsernameSettings { get; private set; } = new();
    public PasswordSettings PasswordSettings { get; private set; } = new();
  }
}
