using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Application;

public abstract record Activity : IActivity
{
  [JsonIgnore]
  private ActivityContext? _context = null;
  [JsonIgnore]
  protected ActivityContext Context => _context ?? throw new InvalidOperationException($"The request has not been contextualized yet. You must call the '{nameof(Contextualize)}' method before executing the request.");

  [JsonIgnore]
  public Actor Actor
  {
    get
    {
      if (Context.User != null)
      {
        return new Actor(Context.User);
      }
      else if (Context.ApiKey != null)
      {
        return new Actor(Context.ApiKey);
      }
      return Actor.System;
    }
  }
  [JsonIgnore]
  public ActorId ActorId => new(Actor.Id);

  [JsonIgnore]
  private Configuration Configuration => Context.Configuration;

  [JsonIgnore]
  public Realm? Realm => Context.Realm;
  [JsonIgnore]
  public TenantId? TenantId => Realm?.GetTenantId();

  [JsonIgnore]
  public LocaleUnit? DefaultLocale
  {
    get
    {
      Locale? defaultLocale = Realm?.DefaultLocale ?? Configuration.DefaultLocale;
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
