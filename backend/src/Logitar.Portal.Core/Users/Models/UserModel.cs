using Logitar.Portal.Core.Realms.Models;

namespace Logitar.Portal.Core.Users.Models
{
  public class UserModel : AggregateModel
  {
    public RealmModel? Realm { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }
    public DateTime? EmailConfirmedAt { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? PhoneNumberConfirmedAt { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }
    public bool IsAccountConfirmed { get; set; }

    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }

    public string? Locale { get; set; }
    public string? Picture { get; set; }

    public DateTime? PasswordChangedAt { get; set; }
    public DateTime? SignedInAt { get; set; }

    public DateTime? DisabledAt { get; set; }
    public bool IsDisabled { get; set; }

    public IEnumerable<ExternalProviderModel> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProviderModel>();
  }
}
