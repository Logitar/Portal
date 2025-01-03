﻿using Logitar.Identity.Core;
using Logitar.Identity.Core.Roles;
using Logitar.Identity.Core.Settings;
using Logitar.Identity.Core.Users;
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

  [Theory(DisplayName = "It should create a new user.")]
  [InlineData(null)]
  [InlineData("53266546-c722-488a-becc-55387a7b9b8d")]
  public async Task It_should_create_a_new_user(string? idValue)
  {
    SetRealm();

    Role role = new(new UniqueName(Realm.UniqueNameSettings, "manage_sales"), actorId: null, RoleId.NewId(TenantId));
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
    if (idValue != null)
    {
      payload.Id = Guid.Parse(idValue);
    }
    payload.CustomAttributes.Add(new("JobTitle", "Sales Manager"));
    payload.CustomIdentifiers.Add(new("HealthInsuranceNumber", Faker.Person.BuildHealthInsuranceNumber()));
    payload.Roles.Add("  Manage_Sales  ");
    CreateUserCommand command = new(payload);
    UserModel user = await ActivityPipeline.ExecuteAsync(command);

    if (payload.Id.HasValue)
    {
      Assert.Equal(payload.Id.Value, user.Id);
    }
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

    RoleModel userRole = Assert.Single(user.Roles);
    Assert.Equal(role.EntityId.ToGuid(), userRole.Id);
  }

  [Fact(DisplayName = "It should throw CustomIdentifierAlreadyUsedException when a custom identifier is already used.")]
  public async Task It_should_throw_CustomIdentifierAlreadyUsedException_when_a_custom_identifier_is_already_used()
  {
    CustomIdentifier healthInsuranceNumber = new(Faker.Person.BuildHealthInsuranceNumber());

    User user = (await _userRepository.LoadAsync()).Single();
    user.SetCustomIdentifier(new Identifier("HealthInsuranceNumber"), healthInsuranceNumber);
    await _userRepository.SaveAsync(user);

    CreateUserPayload payload = new(Faker.Internet.UserName());
    payload.CustomIdentifiers.Add(new("HealthInsuranceNumber", healthInsuranceNumber.Value));
    CreateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<CustomIdentifierAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Null(exception.TenantId);
    Assert.Equal("HealthInsuranceNumber", exception.Key);
    Assert.Equal(healthInsuranceNumber.Value, exception.Value);
  }

  [Fact(DisplayName = "It should throw EmailAddressAlreadyUsedException when the email address is already used.")]
  public async Task It_should_throw_EmailAddressAlreadyUsedException_when_the_email_address_is_already_used()
  {
    CreateUserPayload payload = new(Faker.Person.Email)
    {
      Email = new EmailPayload(Faker.Person.Email, isVerified: false)
    };
    CreateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<EmailAddressAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.Email.Address, exception.EmailAddress);
  }

  [Fact(DisplayName = "It should throw IdAlreadyUsedException when the ID is already taken.")]
  public async Task It_should_throw_IdAlreadyUsedException_when_the_Id_is_already_taken()
  {
    User user = new(new UniqueName(new UniqueNameSettings(), Faker.Internet.UserName()));
    await _userRepository.SaveAsync(user);

    CreateUserPayload payload = new(user.UniqueName.Value)
    {
      Id = user.EntityId.ToGuid()
    };
    CreateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IdAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.Id.Value, exception.Id);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw RolesNotFoundException when some roles cannot be found.")]
  public async Task It_should_throw_RolesNotFoundException_when_some_roles_cannot_be_found()
  {
    CreateUserPayload payload = new(UsernameString);
    payload.Roles.Add("admin");
    CreateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<RolesNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.Roles, exception.Roles);
    Assert.Equal("Roles", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsed when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsed_when_the_unique_name_is_already_used()
  {
    CreateUserPayload payload = new(UsernameString);
    CreateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateUserPayload payload = new($"get_rekt_{Guid.NewGuid()};DROP TABLE users")
    {
      Id = Guid.Empty
    };
    CreateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Id.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "UniqueName");
  }
}
