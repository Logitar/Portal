using Logitar.Portal.Core.Users.Payloads;

namespace Logitar.Portal.Core.Realms.Payloads
{
  public class SaveRealmPayload
  {
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public string? AllowedUsernameCharacters { get; set; }
    public bool RequireConfirmedAccount { get; set; }
    public bool RequireUniqueEmail { get; set; }
    public string? Url { get; set; }

    public Guid? PasswordRecoverySenderId { get; set; }
    public Guid? PasswordRecoveryTemplateId { get; set; }

    public PasswordSettingsPayload? PasswordSettings { get; set; }

    public string? GoogleClientId { get; set; }
  }
}
