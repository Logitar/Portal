using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Users.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class RemoveUserIdentifierCommandTests : IntegrationTests
{
  private const string Key = "HealthInsuranceNumber";

  private readonly IUserRepository _userRepository;

  public RemoveUserIdentifierCommandTests() : base()
  {
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should remove the user identifier.")]
  public async Task It_should_remove_the_user_identifier()
  {
    string healthInsuranceNumber = Faker.Person.BuildHealthInsuranceNumber();

    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    user.SetCustomIdentifier(Key, healthInsuranceNumber);
    await _userRepository.SaveAsync(user);

    RemoveUserIdentifierCommand command = new(user.Id.ToGuid(), Key);
    User? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(result);
    Assert.DoesNotContain(result.CustomIdentifiers, id => id.Key == Key);
  }

  [Fact(DisplayName = "It should return null when the user cannot be found.")]
  public async Task It_should_return_null_when_the_user_cannot_be_found()
  {
    RemoveUserIdentifierCommand command = new(Guid.NewGuid(), Key);
    User? user = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(user);
  }

  [Fact(DisplayName = "It should return null when the user is not in the realm.")]
  public async Task It_should_return_null_when_the_user_is_not_in_the_realm()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    SetRealm();

    RemoveUserIdentifierCommand command = new(user.Id.ToGuid(), Key);
    User? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }
}
