using Logitar.Portal.Domain.Users.Events;
using System.Globalization;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class UserEntity : AggregateEntity
  {
    public UserEntity(UserCreatedEvent @event, Actor actor, RealmEntity? realm = null) : base(@event, actor)
    {
      Username = @event.Username;
      PasswordHash = @event.PasswordHash;
      PasswordChangedOn = @event.PasswordHash == null ? null : @event.OccurredOn;

      Email = @event.Email;
      EmailConfirmedById = @event.IsEmailConfirmed ? @event.ActorId.Value : null;
      EmailConfirmedBy = @event.IsEmailConfirmed ? actor.Serialize() : null;
      EmailConfirmedOn = @event.IsEmailConfirmed ? @event.OccurredOn : null;
      PhoneNumber = @event.PhoneNumber;
      PhoneNumberConfirmedById = @event.IsPhoneNumberConfirmed ? @event.ActorId.Value : null;
      PhoneNumberConfirmedBy = @event.IsPhoneNumberConfirmed ? actor.Serialize() : null;
      PhoneNumberConfirmedOn = @event.IsPhoneNumberConfirmed ? @event.OccurredOn : null;

      FirstName = @event.FirstName;
      MiddleName = @event.MiddleName;
      LastName = @event.LastName;
      FullName = @event.FullName;

      Locale = @event.Locale;
      Picture = @event.Picture;

      Realm = realm;
      RealmId = realm?.RealmId;
    }
    private UserEntity() : base()
    {
    }

    public int UserId { get; private set; }

    public RealmEntity? Realm { get; private set; }
    public int? RealmId { get; private set; }

    public string Username { get; private set; } = string.Empty;
    public string UsernameNormalized
    {
      get => Username.ToUpper();
      private set { }
    }
    public string? PasswordHash { get; private set; }
    public DateTime? PasswordChangedOn { get; private set; }
    public bool HasPassword
    {
      get => PasswordHash != null && PasswordChangedOn.HasValue;
      private set { }
    }

    public string? Email { get; private set; }
    public string? EmailNormalized
    {
      get => Email?.ToUpper();
      private set { }
    }
    public string? EmailConfirmedById { get; private set; }
    public string? EmailConfirmedBy { get; private set; }
    public DateTime? EmailConfirmedOn { get; private set; }
    public bool IsEmailConfirmed
    {
      get => EmailConfirmedById != null && EmailConfirmedOn.HasValue;
      private set { }
    }
    public string? PhoneNumber { get; private set; }
    public string? PhoneNumberNormalized
    {
      get => PhoneNumber?.ToUpper();
      private set { }
    }
    public string? PhoneNumberConfirmedById { get; private set; }
    public string? PhoneNumberConfirmedBy { get; private set; }
    public DateTime? PhoneNumberConfirmedOn { get; private set; }
    public bool IsPhoneNumberConfirmed
    {
      get => PhoneNumberConfirmedById != null && PhoneNumberConfirmedOn.HasValue;
      private set { }
    }
    public bool IsAccountConfirmed
    {
      get => IsEmailConfirmed || IsPhoneNumberConfirmed;
      private set { }
    }

    public string? FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string? LastName { get; private set; }
    public string? FullName { get; private set; }

    public CultureInfo? Locale { get; private set; }
    public string? Picture { get; private set; }

    public DateTime? SignedInOn { get; private set; }

    public string? DisabledById { get; private set; }
    public string? DisabledBy { get; private set; }
    public DateTime? DisabledOn { get; private set; }
    public bool IsDisabled
    {
      get => DisabledById != null && DisabledOn.HasValue;
      private set { }
    }

    public List<ExternalProviderEntity> ExternalProviders { get; private set; } = new();
    public List<SessionEntity> Sessions { get; private set; } = new();

    public override void UpdateActors(string id, Actor actor)
    {
      base.UpdateActors(id, actor);

      if (EmailConfirmedById == id)
      {
        EmailConfirmedBy = actor.Serialize();
      }

      if (PhoneNumberConfirmedById == id)
      {
        PhoneNumberConfirmedBy = actor.Serialize();
      }

      if (DisabledById == id)
      {
        DisabledBy = actor.Serialize();
      }
    }

    public void AddExternalProvider(UserAddedExternalProviderEvent @event)
    {
      ExternalProviders.Add(new ExternalProviderEntity(@event, this, new Actor(this)));
    }
    public void ChangePassword(UserChangedPasswordEvent @event)
    {
      PasswordHash = @event.PasswordHash;
      PasswordChangedOn = @event.OccurredOn;
    }
    public void Disable(UserDisabledEvent @event, Actor actor)
    {
      DisabledById = @event.ActorId.Value;
      DisabledBy = actor.Serialize();
      DisabledOn = @event.OccurredOn;
    }
    public void Enable()
    {
      DisabledById = null;
      DisabledOn = null;
    }
    public void SignIn(UserSignedInEvent @event) => SignedInOn = @event.OccurredOn;
    public void Update(UserUpdatedEvent @event, Actor actor)
    {
      base.Update(@event, actor);

      if (@event.PasswordHash != null)
      {
        PasswordHash = @event.PasswordHash;
        PasswordChangedOn = @event.OccurredOn;
      }

      if (@event.HasEmailChanged)
      {
        Email = @event.Email;
        EmailConfirmedById = null;
        EmailConfirmedOn = null;
      }
      if (@event.HasPhoneNumberChanged)
      {
        PhoneNumber = @event.PhoneNumber;
        PhoneNumberConfirmedById = null;
        PhoneNumberConfirmedOn = null;
      }

      FirstName = @event.FirstName;
      MiddleName = @event.MiddleName;
      LastName = @event.LastName;
      FullName = @event.FullName;

      Locale = @event.Locale;
      Picture = @event.Picture;
    }
  }
}
