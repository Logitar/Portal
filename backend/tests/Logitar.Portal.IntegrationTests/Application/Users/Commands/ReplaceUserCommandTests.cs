using Logitar.Data;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Users.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceUserCommandTests : IntegrationTests
{
  private readonly IPasswordManager _passwordManager;
  private readonly IRoleRepository _roleRepository;
  private readonly IUserRepository _userRepository;

  public ReplaceUserCommandTests() : base()
  {
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
    _roleRepository = ServiceProvider.GetRequiredService<IRoleRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [IdentityDb.Roles.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should replace an existing user.")]
  public async Task It_should_replace_an_existing_user()
  {
    const string newPassword = "Test123!";

    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    RoleAggregate admin = new(new UniqueName(uniqueNameSettings, "admin"));
    RoleAggregate editor = new(new UniqueName(uniqueNameSettings, "editor"));
    RoleAggregate reviewer = new(new UniqueName(uniqueNameSettings, "reviewer"));
    await _roleRepository.SaveAsync([admin, editor, reviewer]);

    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    user.SetCustomAttribute("HourlyRate", "37.50");
    string jobTitle = Faker.Name.JobTitle();
    user.SetCustomAttribute("JobTitle", jobTitle);
    user.Update();
    user.AddRole(admin);
    await _userRepository.SaveAsync(user);
    long version = user.Version;

    user.SetCustomAttribute("HourlyRate", "42.25");
    string jobDescriptor = Faker.Name.JobDescriptor();
    user.SetCustomAttribute("JobDescriptor", jobDescriptor);
    user.Update();
    user.AddRole(editor);
    user.RemoveRole(admin);
    await _userRepository.SaveAsync(user);

    ReplaceUserPayload payload = new(Faker.Internet.UserName())
    {
      Password = newPassword,
      IsDisabled = true,
      Address = new AddressPayload
      {
        Street = "150 Saint-Catherine St W",
        Locality = "Montreal",
        PostalCode = "H2X 3Y2",
        Region = "QC",
        Country = "CA"
      },
      Email = new EmailPayload
      {
        Address = Faker.Person.Email,
        IsVerified = true
      },
      Phone = new PhonePayload
      {
        CountryCode = "CA",
        Number = "+15148454636",
        Extension = "999873"
      },
      FirstName = Faker.Person.FirstName,
      MiddleName = "    ",
      LastName = Faker.Person.LastName,
      Nickname = string.Concat(" ", Faker.Person.FirstName.First(), Faker.Person.LastName, " ").ToLower(),
      Birthdate = Faker.Person.DateOfBirth,
      Gender = Faker.Person.Gender.ToString(),
      Locale = "fr-CA",
      TimeZone = "America/Montreal",
      Picture = Faker.Person.Avatar,
      Profile = $"https://www.desjardins.com/employees/123-456789-0",
      Website = $"https://www.{Faker.Person.Website}/".ToLower()
    };
    jobTitle = Faker.Name.JobTitle();
    payload.CustomAttributes.Add(new("JobTitle", jobTitle));
    payload.CustomAttributes.Add(new("Department", "Finance"));
    payload.Roles.Add(admin.UniqueName.Value);
    payload.Roles.Add(reviewer.UniqueName.Value);
    ReplaceUserCommand command = new(user.Id.ToGuid(), payload, version);
    User? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(result);

    Assert.Equal(payload.UniqueName, result.UniqueName);
    Assert.Equal(payload.IsDisabled, result.IsDisabled);

    Assert.NotNull(result.Address);
    Assert.Equal(payload.Address.Street, result.Address.Street);
    Assert.NotNull(result.Email);
    Assert.Equal(payload.Email.Address, result.Email.Address);
    Assert.NotNull(result.Phone);
    Assert.Equal(payload.Phone.Number, result.Phone.Number);
    Assert.True(result.IsConfirmed);

    Assert.Equal(payload.FirstName, result.FirstName);
    Assert.Null(result.MiddleName);
    Assert.Equal(payload.LastName, result.LastName);
    Assert.Equal(payload.Nickname.Trim(), result.Nickname);
    Assertions.Equal(payload.Birthdate, result.Birthdate, TimeSpan.FromSeconds(1));
    Assert.Equal(payload.Gender, result.Gender, ignoreCase: true);
    Assert.Equal(payload.Locale, result.Locale?.Code);
    Assert.Equal(payload.TimeZone, result.TimeZone);
    Assert.Equal(payload.Picture, result.Picture);
    Assert.Equal(payload.Profile, result.Profile);
    Assert.Equal(payload.Website, result.Website);

    Assert.Equal(3, result.CustomAttributes.Count);
    Assert.Contains(result.CustomAttributes, c => c.Key == "JobTitle" && c.Value == jobTitle);
    Assert.Contains(result.CustomAttributes, c => c.Key == "JobDescriptor" && c.Value == jobDescriptor);
    Assert.Contains(result.CustomAttributes, c => c.Key == "Department" && c.Value == "Finance");

    Assert.Equal(2, result.Roles.Count);
    Assert.Contains(result.Roles, r => r.Id == editor.Id.ToGuid());
    Assert.Contains(result.Roles, r => r.Id == reviewer.Id.ToGuid());

    UserEntity? entity = await IdentityContext.Users.SingleOrDefaultAsync(x => x.AggregateId == user.Id.Value);
    Assert.NotNull(entity);
    Assert.NotNull(entity.PasswordHash);
    Password password = _passwordManager.Decode(entity.PasswordHash);
    Assert.True(password.IsMatch(newPassword));
  }

  [Fact(DisplayName = "It should return null when the user cannot be found.")]
  public async Task It_should_return_null_when_the_user_cannot_be_found()
  {
    ReplaceUserPayload payload = new("admin");
    ReplaceUserCommand command = new(Guid.NewGuid(), payload, Version: null);
    User? user = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(user);
  }

  [Fact(DisplayName = "It should return null when the user is in another tenant.")]
  public async Task It_should_return_null_when_the_user_is_in_another_tenant()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    SetRealm();

    ReplaceUserPayload payload = new("admin");
    ReplaceUserCommand command = new(user.Id.ToGuid(), payload, Version: null);
    User? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw EmailAddressAlreadyUsedException when the email address is already used.")]
  public async Task It_should_throw_EmailAddressAlreadyUsedException_when_the_email_address_is_already_used()
  {
    UserAggregate user = new(new UniqueName(new ReadOnlyUniqueNameSettings(), Faker.Internet.UserName()));
    await _userRepository.SaveAsync(user);

    ReplaceUserPayload payload = new(user.UniqueName.Value)
    {
      Email = new EmailPayload(Faker.Person.Email, isVerified: true)
    };
    ReplaceUserCommand command = new(user.Id.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<EmailAddressAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.Email.Address, exception.Email.Address);
  }

  [Fact(DisplayName = "It should throw RolesNotFoundException when some roles cannot be found.")]
  public async Task It_should_throw_RolesNotFoundException_when_some_roles_cannot_be_found()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    ReplaceUserPayload payload = new(user.UniqueName.Value);
    payload.Roles.Add("admin");
    ReplaceUserCommand command = new(user.Id.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<RolesNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.Roles, exception.Roles);
    Assert.Equal("Roles", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    UserAggregate user = new(new UniqueName(new ReadOnlyUniqueNameSettings(), Faker.Internet.UserName()));
    await _userRepository.SaveAsync(user);

    ReplaceUserPayload payload = new(UsernameString.ToUpper());
    ReplaceUserCommand command = new(user.Id.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<UserAggregate>>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.UniqueName.Value);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceUserPayload payload = new("/!\\");
    ReplaceUserCommand command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("UniqueName", exception.Errors.Single().PropertyName);
  }
}
