using Logitar.EventSourcing;
using Logitar.Identity.Domain;
using Logitar.Portal.Application;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal;

internal class TestApplicationContext : IApplicationContext
{
  public Actor Actor { get; set; } = new();
  public ActorId ActorId => new(Actor.Id);

  public ConfigurationAggregate Configuration { get; } = new(new Locale("en"));
}
