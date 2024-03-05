using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Configurations.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadConfigurationQueryTests : IntegrationTests
{
  private readonly ICacheService _cacheService;

  public ReadConfigurationQueryTests() : base()
  {
    _cacheService = ServiceProvider.GetRequiredService<ICacheService>();
  }

  [Fact(DisplayName = "It should return the configuration when it is cached.")]
  public async Task It_should_return_the_configuration_when_it_is_cached()
  {
    Assert.NotNull(_cacheService.Configuration);

    Configuration? configuration = await Mediator.Send(new ReadConfigurationQuery());
    Assert.NotNull(configuration);
    Assert.Same(_cacheService.Configuration, configuration);
  }

  [Fact(DisplayName = "It should throw InvalidOperationException when the configuration is not cached.")]
  public async Task It_should_throw_InvalidOperationException_when_the_configuration_is_not_cached()
  {
    _cacheService.Configuration = null;
    Assert.Null(_cacheService.Configuration);

    await Assert.ThrowsAsync<InvalidOperationException>(async () => await Mediator.Send(new ReadConfigurationQuery()));
  }
}
