using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Contracts.Users;
using System.Globalization;

namespace Logitar.Portal.Contracts.Realms
{
  public record RealmModel : AggregateModel
  {
    public string Alias { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }

    public CultureInfo? DefaultLocale { get; set; }
    public string? Url { get; set; }

    public bool RequireConfirmedAccount { get; set; }
    public bool RequireUniqueEmail { get; set; }

    public UsernameSettingsModel UsernameSettings { get; set; } = new();
    public PasswordSettingsModel PasswordSettings { get; set; } = new();

    public SenderModel? PasswordRecoverySender { get; set; }
    public TemplateModel? PasswordRecoveryTemplate { get; set; }

    public string JwtSecret { get; set; } = string.Empty;

    public string? GoogleClientId { get; set; }
  }
}
