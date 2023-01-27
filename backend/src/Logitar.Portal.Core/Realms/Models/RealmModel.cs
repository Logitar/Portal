using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Realms.Models
{
  public class RealmModel : AggregateModel
  {
    public string Alias { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }

    public string? DefaultLocale { get; set; }
    public string? Url { get; set; }

    public bool RequireConfirmedAccount { get; set; }
    public bool RequireUniqueEmail { get; set; }

    public UsernameSettings UsernameSettings { get; set; } = null!;
    public PasswordSettings PasswordSettings { get; set; } = null!;

    // TODO(fpion): PasswordRecoverySender
    // TODO(fpion): PasswordRecoveryTemplate

    public string? GoogleClientId { get; set; }
  }
}
