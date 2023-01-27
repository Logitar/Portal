using Logitar.Portal.Core2.Actors.Models;
using Logitar.Portal.Core2.Models;
using Logitar.Portal.Core2.Realms.Models;

namespace Logitar.Portal.Core2.Users.Models
{
  public class UserModel : AggregateModel
  {
    public RealmModel? Realm { get; init; }

    public string Username { get; init; } = null!;
    public DateTime? PasswordChangedOn { get; init; }
    public bool HasPassword { get; init; }

    public string? Email { get; init; }
    public ActorModel? EmailConfirmedBy { get; init; }
    public DateTime? EmailConfirmedOn { get; init; }
    public bool IsEmailConfirmed { get; init; }
    public string? PhoneNumber { get; init; }
    public ActorModel? PhoneNumberConfirmedBy { get; init; }
    public DateTime? PhoneNumberConfirmedOn { get; init; }
    public bool IsPhoneNumberConfirmed { get; init; }
    public bool IsAccountConfirmed { get; init; }

    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }
    public string? FullName { get; init; }

    public string? Locale { get; init; }
    public string? Picture { get; init; }

    public DateTime? SignedInOn { get; init; }

    public ActorModel? DisabledBy { get; init; }
    public DateTime? DisabledOn { get; init; }
    public bool IsDisabled { get; init; }
  }
}
