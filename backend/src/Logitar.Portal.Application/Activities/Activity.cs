using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Application.Activities;

public abstract record Activity : IActivity
{
  [JsonIgnore]
  private ActivityContext? _context = null;
  [JsonIgnore]
  protected ActivityContext Context => _context ?? throw new InvalidOperationException($"The request has not been contextualized yet. You must call the '{nameof(Contextualize)}' method before executing the request.");

  [JsonIgnore]
  public ActorModel Actor
  {
    get
    {
      if (Context.User != null)
      {
        return new ActorModel(Context.User);
      }
      else if (Context.ApiKey != null)
      {
        return new ActorModel(Context.ApiKey);
      }
      return ActorModel.System;
    }
  }
  [JsonIgnore]
  public ActorId ActorId => new(Actor.Id);

  [JsonIgnore]
  private ConfigurationModel Configuration => Context.Configuration;

  [JsonIgnore]
  public RealmModel? Realm => Context.Realm;
  [JsonIgnore]
  public TenantId? TenantId => Realm?.GetTenantId();

  [JsonIgnore]
  public LocaleUnit? DefaultLocale
  {
    get
    {
      LocaleModel? defaultLocale = Realm?.DefaultLocale ?? Configuration.DefaultLocale;
      return defaultLocale == null ? null : new LocaleUnit(defaultLocale.Code);
    }
  }
  [JsonIgnore]
  public string Secret => Realm?.Secret ?? Configuration.Secret;

  [JsonIgnore]
  public IRoleSettings RoleSettings => new RoleSettings
  {
    UniqueName = Realm?.UniqueNameSettings ?? Configuration.UniqueNameSettings
  };
  [JsonIgnore]
  public IUserSettings UserSettings
  {
    get
    {
      return new UserSettings
      {
        UniqueName = Realm?.UniqueNameSettings ?? Configuration.UniqueNameSettings,
        Password = Realm?.PasswordSettings ?? Configuration.PasswordSettings,
        RequireUniqueEmail = Realm?.RequireUniqueEmail ?? Configuration.RequireUniqueEmail
      };
    }
  }
  [JsonIgnore]
  public bool RequireUniqueEmail => Realm?.RequireUniqueEmail ?? Configuration.RequireUniqueEmail;

  public virtual IActivity Anonymize()
  {
    return this;
  }

  public virtual void Contextualize(ActivityContext context)
  {
    _context = context;
  }
}
