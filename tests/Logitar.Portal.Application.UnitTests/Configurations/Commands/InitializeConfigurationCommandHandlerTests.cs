using Bogus;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations;
using Moq;

namespace Logitar.Portal.Application.Configurations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class InitializeConfigurationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IConfigurationRepository> _configurationRepository = new();
  private readonly Mock<IPasswordService> _passwordService = new();
  private readonly Mock<IUserManager> _userManager = new();

  private readonly InitializeConfigurationCommandHandler _handler;

  public InitializeConfigurationCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _configurationRepository.Object,
      _passwordService.Object, _userManager.Object);
  }

  [Fact(DisplayName = "It should throw ConfigurationAlreadyInitializedException when configuration has already been initialized.")]
  public async Task It_should_throw_ConfigurationAlreadyInitializedException_when_configuration_has_already_been_initialized()
  {
    ConfigurationAggregate configuration = new(CultureInfo.GetCultureInfo(_faker.Locale), new ActorId());
    _configurationRepository.Setup(x => x.LoadAsync(_cancellationToken)).ReturnsAsync(configuration);

    InitializeConfigurationCommand command = new(new InitializeConfigurationPayload());
    await Assert.ThrowsAsync<ConfigurationAlreadyInitializedException>(
      async () => await _handler.Handle(command, _cancellationToken)
    );
  }
}
