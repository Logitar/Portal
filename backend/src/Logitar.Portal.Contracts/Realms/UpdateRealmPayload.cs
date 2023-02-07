using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Realms
{
  public record UpdateRealmPayload
  {
    public string? DisplayName { get; set; }
    public string? Description { get; set; }

    public string? DefaultLocale { get; set; }
    public string? Url { get; set; }

    public bool RequireConfirmedAccount { get; set; }
    public bool RequireUniqueEmail { get; set; }

    public UsernameSettingsPayload UsernameSettings { get; set; } = new();
    public PasswordSettingsPayload PasswordSettings { get; set; } = new();

    public string? PasswordRecoverySenderId { get; set; }
    public string? PasswordRecoveryTemplateId { get; set; }

    public string JwtSecret { get; set; } = string.Empty;

    public string? GoogleClientId { get; set; }
  }
}
