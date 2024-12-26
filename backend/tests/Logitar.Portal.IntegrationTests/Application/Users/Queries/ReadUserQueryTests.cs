using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Users.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadUserQueryTests : IntegrationTests
{
  private readonly IUserRepository _userRepository;

  public ReadUserQueryTests() : base()
  {
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should return null when the user cannot be found.")]
  public async Task It_should_return_null_when_the_user_cannot_be_found()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    SetRealm();

    ReadUserQuery query = new(user.Id.ToGuid(), UniqueName: null, Identifier: null);
    UserModel? found = await ActivityPipeline.ExecuteAsync(query);
    Assert.Null(found);
  }

  [Fact(DisplayName = "It should return the user found by ID.")]
  public async Task It_should_return_the_user_found_by_Id()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    ReadUserQuery query = new(user.Id.ToGuid(), user.UniqueName.Value, Identifier: null);
    UserModel? found = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(found);
    Assert.Equal(user.Id.ToGuid(), found.Id);
  }

  [Fact(DisplayName = "It should return the user found by custom identifier.")]
  public async Task It_should_return_the_user_found_by_custom_identifier()
  {
    const string key = "HealthInsuranceNumber";
    string healthInsuranceNumber = Faker.Person.BuildHealthInsuranceNumber();

    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    user.SetCustomIdentifier(key, healthInsuranceNumber);
    await _userRepository.SaveAsync(user);

    ReadUserQuery query = new(Id: null, UniqueName: null, Identifier: new CustomIdentifier($" {key} ", $"  {healthInsuranceNumber}  "));
    UserModel? found = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(found);
    Assert.Equal(user.Id.ToGuid(), found.Id);
  }

  [Fact(DisplayName = "It should return the user found by email address.")]
  public async Task It_should_return_the_user_found_by_email_address()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    Assert.NotNull(user.Email);

    ReadUserQuery query = new(Id: null, user.Email.Address, Identifier: null);
    UserModel? found = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(found);
    Assert.Equal(user.Id.ToGuid(), found.Id);
  }

  [Fact(DisplayName = "It should return the user found by unique name.")]
  public async Task It_should_return_the_user_found_by_unique_name()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    ReadUserQuery query = new(Id: null, user.UniqueName.Value, Identifier: null);
    UserModel? found = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(found);
    Assert.Equal(user.Id.ToGuid(), found.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when there are too many results.")]
  public async Task It_should_throw_TooManyResultsException_when_there_are_too_many_results()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    UserAggregate other = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), Faker.Internet.UserName()));
    await _userRepository.SaveAsync(other);

    ReadUserQuery query = new(user.Id.ToGuid(), $"  {other.UniqueName.Value}  ", Identifier: null);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<UserModel>>(async () => await ActivityPipeline.ExecuteAsync(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
