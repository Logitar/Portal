using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.Domain.Users.Events;
using Logitar.Portal.Domain.Users.Validators;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Users;

public class UserAggregate : AggregateRoot
{
  private Password? _password = null;

  private EmailAddress? _email = null;

  private string? _firstName = null;
  private string? _middleName = null;
  private string? _lastName = null;
  private string? _nickname = null;

  private Locale? _locale = null;

  public UserAggregate(AggregateId id) : base(id)
  {
  }

  public UserAggregate(IUniqueNameSettings uniqueNameSettings, string uniqueName, string? tenantId = null, ActorId actorId = default, AggregateId? id = null)
    : base(id)
  {
    uniqueName = uniqueName.Trim();
    new UniqueNameValidator(uniqueNameSettings, nameof(UniqueName)).ValidateAndThrow(uniqueName);

    tenantId = tenantId?.CleanTrim();
    if (tenantId != null)
    {
      new TenantIdValidator(nameof(TenantId)).ValidateAndThrow(tenantId);
    }

    ApplyChange(new UserCreatedEvent(actorId)
    {
      TenantId = tenantId,
      UniqueName = uniqueName
    });
  }
  protected virtual void Apply(UserCreatedEvent created)
  {
    TenantId = created.TenantId;

    UniqueName = created.UniqueName;
  }

  public string? TenantId { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public bool HasPassword => _password != null;
  public bool IsDisabled { get; private set; }
  public DateTime? AuthenticatedOn { get; private set; }

  public EmailAddress? Email
  {
    get => _email;
    set
    {
      if (value != _email)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.Email = new Modification<EmailAddress>(value);
        _email = value;
      }
    }
  }
  public bool IsConfirmed => Email?.IsVerified == true;

  public string? FirstName
  {
    get => _firstName;
    set
    {
      value = value?.CleanTrim();
      if (value != null)
      {
        new PersonNameValidator(nameof(FirstName)).ValidateAndThrow(value);
      }

      if (value != _firstName)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.FirstName = new Modification<string>(value);
        updated.FullName = new Modification<string>(PersonHelper.BuildFullName(value, MiddleName, LastName));
        _firstName = value;
      }
    }
  }
  public string? MiddleName
  {
    get => _middleName;
    set
    {
      value = value?.CleanTrim();
      if (value != null)
      {
        new PersonNameValidator(nameof(MiddleName)).ValidateAndThrow(value);
      }

      if (value != _middleName)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.MiddleName = new Modification<string>(value);
        updated.FullName = new Modification<string>(PersonHelper.BuildFullName(FirstName, value, LastName));
        _middleName = value;
      }
    }
  }
  public string? LastName
  {
    get => _lastName;
    set
    {
      value = value?.CleanTrim();
      if (value != null)
      {
        new PersonNameValidator(nameof(LastName)).ValidateAndThrow(value);
      }

      if (value != _lastName)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.LastName = new Modification<string>(value);
        updated.FullName = new Modification<string>(PersonHelper.BuildFullName(FirstName, MiddleName, value));
        _lastName = value;
      }
    }
  }
  public string? FullName { get; private set; }
  public string? Nickname
  {
    get => _nickname;
    set
    {
      value = value?.CleanTrim();
      if (value != null)
      {
        new PersonNameValidator(nameof(Nickname)).ValidateAndThrow(value);
      }

      if (value != _nickname)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.Nickname = new Modification<string>(value);
        _nickname = value;
      }
    }
  }

  public Locale? Locale
  {
    get => _locale;
    set
    {
      if (value != _locale)
      {
        UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
        updated.Locale = new Modification<Locale>(value);
        _locale = value;
      }
    }
  }

  public void Disable(ActorId actorId = default)
  {
    if (!IsDisabled)
    {
      ApplyChange(new UserDisabledEvent(actorId));
    }
  }
  protected virtual void Apply(UserDisabledEvent _) => IsDisabled = true;

  public void Enable(ActorId actorId = default)
  {
    if (IsDisabled)
    {
      ApplyChange(new UserEnabledEvent(actorId));
    }
  }
  protected virtual void Apply(UserEnabledEvent _) => IsDisabled = false;

  public void SetPassword(Password password)
  {
    UserUpdatedEvent updated = GetLatestEvent<UserUpdatedEvent>();
    updated.Password = password;
    _password = password;
  }

  public SessionAggregate SignIn(IUserSettings userSettings, Password? secret = null, ActorId? actorId = null)
  {
    return SignIn(userSettings, password: null, secret, actorId);
  }
  public SessionAggregate SignIn(IUserSettings userSettings, string? password, Password? secret = null, ActorId? actorId = null)
  {
    if (password != null && _password?.IsMatch(password) != true)
    {
      throw new IncorrectUserPasswordException(this, password);
    }

    if (IsDisabled)
    {
      throw new UserIsDisabledException(this);
    }
    else if (userSettings.RequireConfirmedAccount && !IsConfirmed)
    {
      throw new UserIsNotConfirmedException(this);
    }

    actorId ??= new(Id.Value);

    SessionAggregate session = new(this, secret, actorId.Value);

    ApplyChange(new UserSignedInEvent(actorId.Value, session.CreatedOn));

    return session;
  }
  protected virtual void Apply(UserSignedInEvent signedIn) => AuthenticatedOn = signedIn.OccurredOn;

  public void Update(ActorId actorId)
  {
    foreach (DomainEvent change in Changes)
    {
      if (change is UserUpdatedEvent updated && updated.ActorId == default)
      {
        updated.ActorId = actorId;

        if (updated.Version == Version)
        {
          UpdatedBy = actorId;
        }
      }
    }
  }

  protected virtual void Apply(UserUpdatedEvent updated)
  {
    if (updated.Password != null)
    {
      _password = updated.Password;
    }

    if (updated.Email != null)
    {
      _email = updated.Email.Value;
    }

    if (updated.FirstName != null)
    {
      _firstName = updated.FirstName.Value;
    }
    if (updated.MiddleName != null)
    {
      _middleName = updated.MiddleName.Value;
    }
    if (updated.LastName != null)
    {
      _lastName = updated.LastName.Value;
    }
    if (updated.FullName != null)
    {
      FullName = updated.FullName.Value;
    }
    if (updated.Nickname != null)
    {
      Nickname = updated.Nickname.Value;
    }

    if (updated.Locale != null)
    {
      _locale = updated.Locale.Value;
    }
  }

  protected virtual T GetLatestEvent<T>() where T : DomainEvent, new()
  {
    T? updated = Changes.SingleOrDefault(change => change is T) as T;
    if (updated == null)
    {
      updated = new();
      ApplyChange(updated);
    }

    return updated;
  }

  public override string ToString() => $"{FullName ?? UniqueName} | {base.ToString()}";
}
