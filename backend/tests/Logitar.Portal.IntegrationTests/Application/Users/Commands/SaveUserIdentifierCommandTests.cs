using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Contracts.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Users.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SaveUserIdentifierCommandTests : IntegrationTests
{
  private static readonly Identifier _key = new("HealthInsuranceNumber");

  private readonly IUserRepository _userRepository;

  private readonly CustomIdentifier _healthInsuranceNumber;

  public SaveUserIdentifierCommandTests() : base()
  {
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();

    _healthInsuranceNumber = new(Faker.Person.BuildHealthInsuranceNumber());
  }

  [Fact(DisplayName = "It should save the user identifier.")]
  public async Task It_should_save_the_user_identifier()
  {
    User user = Assert.Single(await _userRepository.LoadAsync());
    user.SetCustomIdentifier(_key, new CustomIdentifier("old_value"));
    await _userRepository.SaveAsync(user);

    SaveUserIdentifierPayload payload = new(_healthInsuranceNumber.Value);
    SaveUserIdentifierCommand command = new(user.EntityId.ToGuid(), _key.Value, payload);
    UserModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(result);
    Assert.Contains(result.CustomIdentifiers, id => id.Key == command.Key && id.Value == payload.Value);
  }

  [Fact(DisplayName = "It should return null when the user cannot be found.")]
  public async Task It_should_return_null_when_the_user_cannot_be_found()
  {
    SaveUserIdentifierPayload payload = new(_healthInsuranceNumber.Value);
    SaveUserIdentifierCommand command = new(Guid.NewGuid(), _key.Value, payload);
    UserModel? user = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(user);
  }

  [Fact(DisplayName = "It should return null when the user is not in the realm.")]
  public async Task It_should_return_null_when_the_user_is_not_in_the_realm()
  {
    User user = Assert.Single(await _userRepository.LoadAsync());

    SetRealm();

    SaveUserIdentifierPayload payload = new(_healthInsuranceNumber.Value);
    SaveUserIdentifierCommand command = new(user.EntityId.ToGuid(), _key.Value, payload);
    UserModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw CustomIdentifierAlreadyUsedException when the identifier is already used.")]
  public async Task It_should_throw_CustomIdentifierAlreadyUsedException_when_the_identifier_is_already_used()
  {
    User user = Assert.Single(await _userRepository.LoadAsync());

    User other = new(new UniqueName(Realm.UniqueNameSettings, Faker.Internet.UserName()));
    other.SetCustomIdentifier(_key, _healthInsuranceNumber);
    await _userRepository.SaveAsync(other);

    SaveUserIdentifierPayload payload = new(_healthInsuranceNumber.Value);
    SaveUserIdentifierCommand command = new(user.EntityId.ToGuid(), _key.Value, payload);
    var exception = await Assert.ThrowsAsync<CustomIdentifierAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(command.Key, exception.Key);
    Assert.Equal(payload.Value, exception.Value);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SaveUserIdentifierPayload payload = new(value: string.Empty);
    SaveUserIdentifierCommand command = new(Guid.NewGuid(), Key: string.Empty, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.PropertyName == "Key");
    Assert.Contains(exception.Errors, e => e.PropertyName == "Value");
  }
}
