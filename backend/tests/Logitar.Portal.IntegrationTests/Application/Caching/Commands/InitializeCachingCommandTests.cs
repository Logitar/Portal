using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Configurations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Caching.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class InitializeCachingCommandTests : IntegrationTests
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IPublisher _publisher;

  public InitializeCachingCommandTests() : base(initializeConfiguration: false)
  {
    _cacheService = ServiceProvider.GetRequiredService<ICacheService>();
    _configurationRepository = ServiceProvider.GetRequiredService<IConfigurationRepository>();
    _publisher = ServiceProvider.GetRequiredService<IPublisher>();
  }

  [Fact(DisplayName = "It should cache the configuration if it has been initialized.")]
  public async Task It_should_cache_the_configuration_if_it_has_been_initialized()
  {
    LocaleUnit defaultLocale = new(Faker.Locale);
    ActorId actorId = ActorId.NewId();
    ConfigurationAggregate configuration = ConfigurationAggregate.Initialize(defaultLocale, actorId);
    await _configurationRepository.SaveAsync(configuration);
    Assert.NotNull(_cacheService.Configuration);
  }

  [Fact(DisplayName = "It should not cache the configuration if it has not been initialized.")]
  public async Task It_should_not_cache_the_configuration_if_it_has_not_been_initialized()
  {
    await _publisher.Publish(new InitializeCachingCommand());
    Assert.Null(_cacheService.Configuration);
  }
}
