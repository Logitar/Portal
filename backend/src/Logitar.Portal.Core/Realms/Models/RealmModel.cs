using Logitar.Portal.Core.Users.Models;

namespace Logitar.Portal.Core.Realms.Models
{
  public class RealmModel : AggregateModel
  {
    public string Alias { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public string? AllowedUsernameCharacters { get; set; }
    public bool RequireConfirmedAccount { get; set; }
    public bool RequireUniqueEmail { get; set; }
    public string? Url { get; set; }

    public Guid? PasswordRecoverySenderId { get; set; }
    public Guid? PasswordRecoveryTemplateId { get; set; }

    public PasswordSettingsModel? PasswordSettings { get; set; }

    public string? GoogleClientId { get; set; }
  }
}
