using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application;

public abstract record ApplicationRequest
{
  [JsonIgnore]
  public Actor Actor { get; private set; }
  [JsonIgnore]
  public ActorId ActorId => new(Actor.Id);

  private Configuration? _configuration = null;
  [JsonIgnore]
  public Configuration Configuration => _configuration ?? throw new InvalidOperationException($"The {nameof(Configuration)} has not been initialized yet.");
  [JsonIgnore]
  public Realm? Realm { get; private set; }
  [JsonIgnore]
  public LocaleUnit? DefaultLocale => LocaleUnit.TryCreate((Realm?.DefaultLocale ?? Configuration.DefaultLocale)?.Code);
  [JsonIgnore]
  public bool RequireUniqueEmail => Realm?.RequireUniqueEmail ?? Configuration.RequireUniqueEmail;
  [JsonIgnore]
  public TenantId? TenantId => Realm == null ? null : new(new RealmId(Realm.Id).Value);

  protected ApplicationRequest()
  {
    Actor = Actor.System;
  }

  public void Populate(Actor actor, Configuration configuration, Realm? realm)
  {
    Actor = actor;
    _configuration = configuration;
    Realm = realm;
  }
}
