using Logitar.Portal.Core2.Models;

namespace Logitar.Portal.Core2.Realms.Models
{
  public class RealmModel : AggregateModel
  {
    public string Alias { get; init; } = null!;
    public string DisplayName { get; init; } = null!;
    public string? Description { get; init; }

    public string? DefaultLocale { get; init; }
    public string? Url { get; init; }

    public bool RequireConfirmedAccount { get; init; }
    public bool RequireUniqueEmail { get; init; }

    // TODO(fpion): UsernameSettings
    // TODO(fpion): PasswordSettings

    // TODO(fpion): PasswordRecoverySender
    // TODO(fpion): PasswordRecoveryTemplate

    public string? GoogleClientId { get; init; }
  }
}
