using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Users.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateUserCommandTests : IntegrationTests
{
  private readonly IRoleRepository _roleRepository;
  private readonly IUserRepository _userRepository;

  public CreateUserCommandTests() : base()
  {
    _roleRepository = ServiceProvider.GetRequiredService<IRoleRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should create a new user.")]
  public async Task It_should_create_a_new_user()
  {
    SetRealm();

    RoleAggregate role = new(new UniqueNameUnit(Realm.UniqueNameSettings, "manage_sales"), TenantId);
    await _roleRepository.SaveAsync(role);

    CreateUserPayload payload = new(UsernameString)
    {
      Password = PasswordString,
      IsDisabled = true,
      Email = new EmailPayload(Faker.Person.Email, isVerified: true),
      FirstName = Faker.Person.FirstName,
      LastName = Faker.Person.LastName,
      Birthdate = Faker.Person.DateOfBirth,
      Gender = Faker.Person.Gender.ToString(),
      Picture = Faker.Person.Avatar
    };
    payload.CustomAttributes.Add(new("JobTitle", "Sales Manager"));
    payload.CustomIdentifiers.Add(new("HealthInsuranceNumber", Faker.Person.BuildHealthInsuranceNumber()));
    payload.Roles.Add("  Manage_Sales  ");
    CreateUserCommand command = new(payload);
    User user = await Mediator.Send(command);

    Assert.Equal(payload.UniqueName, user.UniqueName);
    Assert.True(user.HasPassword);
    Assert.True(user.IsDisabled);
    Assert.NotNull(user.Email);
    Assert.Equal(payload.Email.Address, user.Email.Address);
    Assert.True(user.IsConfirmed);
    Assert.Equal(payload.FirstName, user.FirstName);
    Assert.Equal(payload.LastName, user.LastName);
    Assertions.Equal(payload.Birthdate, user.Birthdate, TimeSpan.FromSeconds(1));
    Assert.Equal(payload.Gender, user.Gender, ignoreCase: true);
    Assert.Equal(payload.Picture, user.Picture);
    Assert.Equal(payload.CustomAttributes, user.CustomAttributes);
    Assert.Equal(payload.CustomIdentifiers, user.CustomIdentifiers);
    Assert.Same(Realm, user.Realm);

    Role userRole = Assert.Single(user.Roles);
    Assert.Equal(role.Id.ToGuid(), userRole.Id);
  }

  [Fact(DisplayName = "It should throw CustomIdentifierAlreadyUsedException when a custom identifier is already used.")]
  public async Task It_should_throw_CustomIdentifierAlreadyUsedException_when_a_custom_identifier_is_already_used()
  {
    string healthInsuranceNumber = Faker.Person.BuildHealthInsuranceNumber();

    UserAggregate user = (await _userRepository.LoadAsync()).Single();
    user.SetCustomIdentifier("HealthInsuranceNumber", healthInsuranceNumber);
    await _userRepository.SaveAsync(user);

    CreateUserPayload payload = new(Faker.Internet.UserName());
    payload.CustomIdentifiers.Add(new("HealthInsuranceNumber", healthInsuranceNumber));
    CreateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<CustomIdentifierAlreadyUsedException<UserAggregate>>(async () => await Mediator.Send(command));
    Assert.Null(exception.TenantId);
    Assert.Equal("HealthInsuranceNumber", exception.Key);
    Assert.Equal(healthInsuranceNumber, exception.Value);
  }

  [Fact(DisplayName = "It should throw EmailAddressAlreadyUsedException when the email address is already used.")]
  public async Task It_should_throw_EmailAddressAlreadyUsedException_when_the_email_address_is_already_used()
  {
    CreateUserPayload payload = new(Faker.Person.Email)
    {
      Email = new EmailPayload(Faker.Person.Email, isVerified: false)
    };
    CreateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<EmailAddressAlreadyUsedException>(async () => await Mediator.Send(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.Email.Address, exception.Email.Address);
  }

  [Fact(DisplayName = "It should throw RolesNotFoundException when some roles cannot be found.")]
  public async Task It_should_throw_RolesNotFoundException_when_some_roles_cannot_be_found()
  {
    CreateUserPayload payload = new(UsernameString);
    payload.Roles.Add("admin");
    CreateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<RolesNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(payload.Roles, exception.Roles);
    Assert.Equal("Roles", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsed when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsed_when_the_unique_name_is_already_used()
  {
    CreateUserPayload payload = new(UsernameString);
    CreateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<UserAggregate>>(async () => await Mediator.Send(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.UniqueName.Value);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateUserPayload payload = new($"get_rekt_{Guid.NewGuid()};DROP TABLE users");
    CreateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("UniqueName", exception.Errors.Single().PropertyName);
  }
}
