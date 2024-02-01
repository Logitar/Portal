using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Sessions.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateSessionCommandTests : IntegrationTests
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserRepository _userRepository;

  public CreateSessionCommandTests() : base()
  {
    _applicationContext = ServiceProvider.GetRequiredService<IApplicationContext>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should create a persistent session.")]
  public async Task It_should_create_a_persistent_session()
  {
    CreateSessionPayload payload = new(Faker.Person.UserName)
    {
      IsPersistent = true
    };
    CreateSessionCommand command = new(payload);
    Session session = await Mediator.Send(command);

    Assert.True(session.IsPersistent);
    Assert.NotNull(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Empty(session.CustomAttributes);

    UserAggregate? user = (await _userRepository.LoadAsync()).SingleOrDefault();
    Assert.NotNull(user);
    Assert.Null(user.TenantId);
    Assert.Equal(user.Id.AggregateId.ToGuid(), session.User.Id);
  }

  [Fact(DisplayName = "It should create a realm session.")]
  public async Task It_should_create_a_realm_session()
  {
    SetRealm();

    UserAggregate user = new(new UniqueNameUnit(Realm.UniqueNameSettings, Faker.Person.UserName), TenantId);
    await _userRepository.SaveAsync(user);

    CreateSessionPayload payload = new(Faker.Person.UserName);
    payload.CustomAttributes.Add(new("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}"));
    payload.CustomAttributes.Add(new("IpAddress", Faker.Internet.Ip()));
    CreateSessionCommand command = new(payload);
    Session session = await Mediator.Send(command);

    Assert.False(session.IsPersistent);
    Assert.Null(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(payload.CustomAttributes, session.CustomAttributes);
    Assert.Equal(user.Id.AggregateId.ToGuid(), session.User.Id);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when the user could not be found.")]
  public async Task It_should_throw_UserNotFoundException_when_the_user_could_not_be_found()
  {
    SetRealm();

    CreateSessionPayload payload = new(Faker.Person.UserName);
    CreateSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(_applicationContext.TenantId, exception.TenantId);
    Assert.Equal(payload.User, exception.User);
    Assert.Equal("User", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateSessionPayload payload = new(user: string.Empty);
    CreateSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("User", exception.Errors.Single().PropertyName);
  }
}
