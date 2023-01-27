using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Realms.Payloads
{
  public class UpdateRealmPayload
  {
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }

    public string? DefaultLocale { get; set; }
    public string? Url { get; set; }

    public bool RequireConfirmedAccount { get; set; }
    public bool RequireUniqueEmail { get; set; }

    public UsernameSettings UsernameSettings { get; set; } = null!;
    public PasswordSettings PasswordSettings { get; set; } = null!;

    public string? PasswordRecoverySenderId { get; set; }
    public string? PasswordRecoveryTemplateId { get; set; }

    public string? GoogleClientId { get; set; }
  }
}
