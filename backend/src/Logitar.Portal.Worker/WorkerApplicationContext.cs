﻿using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Worker;

internal class WorkerApplicationContext : IApplicationContext
{
  private readonly ICacheService _cacheService;

  public Actor Actor { get; set; }
  public ActorId ActorId => new(Actor.Id);

  public Uri BaseUri { get; set; }
  public string BaseUrl => BaseUri.ToString();

  public Configuration Configuration => _cacheService.Configuration
    ?? throw new InvalidOperationException("The configuration was not found in the cache.");
  public Realm? Realm { get; set; }
  public TenantId? TenantId => Realm == null ? null : new(new RealmId(Realm.Id).Value);

  public IRoleSettings RoleSettings
  {
    get
    {
      Realm? realm = Realm;
      if (realm != null)
      {
        return new RoleSettings
        {
          UniqueName = realm.UniqueNameSettings
        };
      }

      Configuration configuration = Configuration;
      return new RoleSettings
      {
        UniqueName = configuration.UniqueNameSettings
      };
    }
  }
  public IUserSettings UserSettings
  {
    get
    {
      Realm? realm = Realm;
      if (realm != null)
      {
        return new UserSettings
        {
          UniqueName = realm.UniqueNameSettings,
          Password = realm.PasswordSettings,
          RequireUniqueEmail = realm.RequireUniqueEmail
        };
      }

      Configuration configuration = Configuration;
      return new UserSettings
      {
        UniqueName = configuration.UniqueNameSettings,
        Password = configuration.PasswordSettings,
        RequireUniqueEmail = configuration.RequireUniqueEmail
      };
    }
  }

  public WorkerApplicationContext(ICacheService cacheService, IConfiguration configuration)
  {
    _cacheService = cacheService;

    Actor = Actor.System;

    string baseUrl = configuration.GetValue<string>("BaseUrl") ?? throw new ArgumentException($"The configuration 'BaseUrl' is required.", nameof(configuration));
    BaseUri = new Uri(baseUrl, UriKind.Absolute);
  }
}