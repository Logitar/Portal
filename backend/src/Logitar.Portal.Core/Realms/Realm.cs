using Logitar.Portal.Core.Users;
using System.Globalization;

namespace Logitar.Portal.Core.Realms
{
  internal class Realm : AggregateRoot
  {
    public string Alias { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public string? Description { get; private set; }

    public CultureInfo? DefaultLocale { get; private set; }
    public string? Url { get; private set; }

    public bool RequireConfirmedAccount { get; private set; }
    public bool RequireUniqueEmail { get; private set; }

    public UsernameSettings UsernameSettings { get; private set; } = null!;
    public PasswordSettings PasswordSettings { get; private set; } = null!;

    // TODO(fpion): PasswordRecoverySender
    // TODO(fpion): PasswordRecoveryTemplate

    public string? GoogleClientId { get; private set; }

    public override string ToString() => $"{DisplayName} | {base.ToString()}";
  }
}
