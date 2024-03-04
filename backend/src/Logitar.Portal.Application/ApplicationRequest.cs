using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application;

public abstract record ApplicationRequest : IActivity
{
  [JsonIgnore]
  private ApplicationContext? _context = null;
  [JsonIgnore]
  protected ApplicationContext Context => _context ?? throw new InvalidOperationException($"The request has not been contextualized yet. You must call the '{nameof(Contextualize)}' method before executing the request.");

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
  public Realm? Realm => Context.Realm;
  [JsonIgnore]
  public TenantId? TenantId => Context.Realm == null ? null : new(new RealmId(Context.Realm.Id).Value);

  [JsonIgnore]
  public LocaleUnit? DefaultLocale
  {
    get
    {
      Locale? defaultLocale = Context.Realm?.DefaultLocale ?? Context.Configuration.DefaultLocale;
      return defaultLocale == null ? null : new LocaleUnit(defaultLocale.Code);
    }
  }

  [JsonIgnore]
  public bool RequireUniqueEmail => Context.Realm?.RequireUniqueEmail ?? Context.Configuration.RequireUniqueEmail;

  public virtual void Contextualize(ApplicationContext context)
  {
    _context = context;
  }

  public virtual IActivity GetActivity()
  {
    return this;
  }
}
