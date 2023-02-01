using Logitar.Portal.Contracts.Users.Models;

namespace Logitar.Portal.Contracts.Realms.Models
{
  public class RealmModel : AggregateModel
  {
    public string Alias { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }

    public string? DefaultLocale { get; set; }
    public string? Url { get; set; }

    public bool RequireConfirmedAccount { get; set; }
    public bool RequireUniqueEmail { get; set; }

    public UsernameSettingsModel? UsernameSettings { get; set; }
    public PasswordSettingsModel? PasswordSettings { get; set; }

    public string? PasswordRecoverySenderId { get; set; }
    public string? PasswordRecoveryTemplateId { get; set; }

    public string? GoogleClientId { get; set; }
  }
}
