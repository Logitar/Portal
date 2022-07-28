using Portal.Core.Realms.Models;

namespace Portal.Core.Users.Models
{
  public class UserModel : AggregateModel
  {
    public RealmModel? Realm { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }
    public DateTime? EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? PhoneNumberConfirmed { get; set; }

    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }

    public string? Locale { get; set; }
    public string? Picture { get; set; }

    public DateTime? PasswordChangedAt { get; set; }
    public DateTime? SignedInAt { get; set; }
  }
}
