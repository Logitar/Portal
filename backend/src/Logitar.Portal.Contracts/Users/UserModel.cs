using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logitar.Portal.Contracts.Users
{
  public record UserModel : AggregateModel
  {
    public RealmModel? Realm { get; set; }

    public string Username { get; set; } = string.Empty;
    public DateTime? PasswordChangedOn { get; set; }
    public bool HasPassword { get; set; }

    public string? Email { get; set; }
    public ActorModel? EmailConfirmedBy { get; set; }
    public DateTime? EmailConfirmedOn { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public ActorModel? PhoneNumberConfirmedBy { get; set; }
    public DateTime? PhoneNumberConfirmedOn { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }
    public bool IsAccountConfirmed { get; set; }

    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }

    public string? Locale { get; set; }
    public string? Picture { get; set; }

    public DateTime? SignedInOn { get; set; }

    public ActorModel? DisabledBy { get; set; }
    public DateTime? DisabledOn { get; set; }
    public bool IsDisabled { get; set; }

    public IEnumerable<ExternalProviderModel> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProviderModel>();
  }
}
