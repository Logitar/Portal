using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Users.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateUserCommandTests : IntegrationTests
{
  private readonly IPasswordManager _passwordManager;
  private readonly IRoleRepository _roleRepository;
  private readonly IUserRepository _userRepository;

  public UpdateUserCommandTests() : base()
  {
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
    _roleRepository = ServiceProvider.GetRequiredService<IRoleRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should return null when the user cannot be found.")]
  public async Task It_should_return_null_when_the_user_cannot_be_found()
  {
    UpdateUserPayload payload = new();
    UpdateUserCommand command = new(Guid.NewGuid(), payload);
    UserModel? user = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(user);
  }

  [Fact(DisplayName = "It should return null when the user is in another tenant.")]
  public async Task It_should_return_null_when_the_user_is_in_another_tenant()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    SetRealm();

    UpdateUserPayload payload = new();
    UpdateUserCommand command = new(user.Id.ToGuid(), payload);
    UserModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw EmailAddressAlreadyUsedException when the email address is already used.")]
  public async Task It_should_throw_EmailAddressAlreadyUsedException_when_the_email_address_is_already_used()
  {
    UserAggregate user = new(new UniqueName(new ReadOnlyUniqueNameSettings(), Faker.Internet.UserName()));
    await _userRepository.SaveAsync(user);

    UpdateUserPayload payload = new()
    {
      Email = new Modification<EmailPayload>(new EmailPayload(Faker.Person.Email, isVerified: true))
    };
    UpdateUserCommand command = new(user.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<EmailAddressAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Null(exception.TenantId);
    Assert.NotNull(payload.Email.Value);
    Assert.Equal(payload.Email.Value.Address, exception.Email.Address);
  }

  [Fact(DisplayName = "It should throw RolesNotFoundException when some roles cannot be found.")]
  public async Task It_should_throw_RolesNotFoundException_when_some_roles_cannot_be_found()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    UpdateUserPayload payload = new();
    payload.Roles.Add(new RoleModification("admin"));
    UpdateUserCommand command = new(user.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<RolesNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.Roles.Select(role => role.Role), exception.Roles);
    Assert.Equal("Roles", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    UserAggregate user = new(new UniqueName(new ReadOnlyUniqueNameSettings(), Faker.Internet.UserName()));
    await _userRepository.SaveAsync(user);

    UpdateUserPayload payload = new()
    {
      UniqueName = UsernameString
    };
    UpdateUserCommand command = new(user.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<UserAggregate>>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.UniqueName.Value);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateUserPayload payload = new()
    {
      UniqueName = "/!\\"
    };
    UpdateUserCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("UniqueName", exception.Errors.Single().PropertyName);
  }

  [Fact(DisplayName = "It should update an existing user.")]
  public async Task It_should_update_an_existing_user()
  {
    const string newPassword = "Test123!";

    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    Role admin = new(new UniqueName(uniqueNameSettings, "admin"));
    Role editor = new(new UniqueName(uniqueNameSettings, "editor"));
    Role reviewer = new(new UniqueName(uniqueNameSettings, "reviewer"));
    await _roleRepository.SaveAsync([admin, editor, reviewer]);

    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    user.SetCustomAttribute("HourlyRate", "37.50");
    user.SetCustomAttribute("JobTitle", Faker.Name.JobTitle());
    user.Update();
    user.AddRole(admin);
    user.AddRole(reviewer);
    await _userRepository.SaveAsync(user);

    UpdateUserPayload payload = new()
    {
      Password = new ChangePasswordPayload(newPassword)
      {
        Current = PasswordString
      },
      IsDisabled = true,
      Birthdate = new Modification<DateTime?>(Faker.Person.DateOfBirth),
      Gender = new Modification<string>(Faker.Person.Gender.ToString()),
      Locale = new Modification<string>("fr-CA"),
      TimeZone = new Modification<string>("America/Montreal"),
      Picture = new Modification<string>(Faker.Person.Avatar),
      Profile = new Modification<string>(value: null),
      Website = new Modification<string>($"https://www.{Faker.Person.Website}/")
    };
    payload.CustomAttributes.Add(new("BaseSalary", "90000"));
    payload.CustomAttributes.Add(new("JobTitle", "Sales Manager"));
    payload.CustomAttributes.Add(new("HourlyRate", value: null));
    payload.Roles.Add(new(editor.UniqueName.Value, CollectionAction.Add));
    payload.Roles.Add(new(admin.UniqueName.Value, CollectionAction.Remove));
    UpdateUserCommand command = new(user.Id.ToGuid(), payload);
    UserModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(result);

    Assert.NotNull(result.PasswordChangedBy);
    Assert.Equal(user.Id.ToGuid(), result.PasswordChangedBy.Id);
    Assert.Equal(payload.IsDisabled.Value, result.IsDisabled);
    Assertions.Equal(payload.Birthdate.Value, result.Birthdate, TimeSpan.FromSeconds(1));
    Assert.Equal(payload.Gender.Value?.ToLower(), result.Gender);
    Assert.Equal(payload.Locale.Value, result.Locale?.Code);
    Assert.Equal(payload.TimeZone.Value, result.TimeZone);
    Assert.Equal(payload.Picture.Value, result.Picture);
    Assert.Null(result.Profile);
    Assert.Equal(payload.Website.Value, result.Website);

    Assert.Equal(2, result.CustomAttributes.Count);
    Assert.Contains(result.CustomAttributes, c => c.Key == "BaseSalary" && c.Value == "90000");
    Assert.Contains(result.CustomAttributes, c => c.Key == "JobTitle" && c.Value == "Sales Manager");

    Assert.Equal(2, result.Roles.Count);
    Assert.Contains(result.Roles, r => r.Id == editor.Id.ToGuid());
    Assert.Contains(result.Roles, r => r.Id == reviewer.Id.ToGuid());

    UserEntity? entity = await IdentityContext.Users.SingleOrDefaultAsync(x => x.AggregateId == user.Id.Value);
    Assert.NotNull(entity);
    Assert.NotNull(entity.PasswordHash);
    Password password = _passwordManager.Decode(entity.PasswordHash);
    Assert.True(password.IsMatch(newPassword));
  }
}
