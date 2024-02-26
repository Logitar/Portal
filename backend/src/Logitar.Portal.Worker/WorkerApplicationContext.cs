using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Worker;

internal class WorkerApplicationContext : IApplicationContext
{
  public Actor Actor => throw new NotSupportedException();
  public ActorId ActorId => throw new NotSupportedException();

  public Uri BaseUri => throw new NotSupportedException();
  public string BaseUrl => throw new NotSupportedException();

  public Configuration Configuration => throw new NotSupportedException();
  public Realm? Realm => throw new NotSupportedException();
  public TenantId? TenantId => throw new NotSupportedException();

  public IRoleSettings RoleSettings => throw new NotSupportedException();
  public IUserSettings UserSettings => throw new NotSupportedException();
}
