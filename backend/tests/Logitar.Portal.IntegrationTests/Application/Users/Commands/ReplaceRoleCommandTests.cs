using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Users.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceUserCommandTests : IntegrationTests
{
  private readonly IPasswordManager _passwordManager;
  private readonly IUserRepository _userRepository;

  public ReplaceUserCommandTests()
  {
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should replace an existing user.")]
  public async Task It_should_replace_an_existing_user()
  {
    const string newPassword = "Test123!";

    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    user.SetCustomAttribute("HourlyRate", "37.50");
    string jobTitle = Faker.Name.JobTitle();
    user.SetCustomAttribute("JobTitle", jobTitle);
    user.Update();
    await _userRepository.SaveAsync(user);
    long version = user.Version;

    user.SetCustomAttribute("HourlyRate", "42.25");
    string jobDescriptor = Faker.Name.JobDescriptor();
    user.SetCustomAttribute("JobDescriptor", jobDescriptor);
    user.Update();
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
    ReplaceUserCommand command = new(user.Id.AggregateId.ToGuid(), payload, version);
    User? result = await Mediator.Send(command);
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
    Assert.Equal(payload.Birthdate?.ToUniversalTime(), result.Birthdate);
    Assert.Equal(payload.Gender, result.Gender, ignoreCase: true);
    Assert.Equal(payload.Locale, result.Locale);
    Assert.Equal(payload.TimeZone, result.TimeZone);
    Assert.Equal(payload.Picture, result.Picture);
    Assert.Equal(payload.Profile, result.Profile);
    Assert.Equal(payload.Website, result.Website);

    Assert.Equal(3, result.CustomAttributes.Count);
    Assert.Contains(result.CustomAttributes, c => c.Key == "JobTitle" && c.Value == jobTitle);
    Assert.Contains(result.CustomAttributes, c => c.Key == "JobDescriptor" && c.Value == jobDescriptor);
    Assert.Contains(result.CustomAttributes, c => c.Key == "Department" && c.Value == "Finance");

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
    User? user = await Mediator.Send(command);
    Assert.Null(user);
  }

  [Fact(DisplayName = "It should return null when the user is in another tenant.")]
  public async Task It_should_return_null_when_the_user_is_in_another_tenant()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    SetRealm();

    ReplaceUserPayload payload = new("admin");
    ReplaceUserCommand command = new(user.Id.AggregateId.ToGuid(), payload, Version: null);
    User? result = await Mediator.Send(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw EmailAddressAlreadyUsedException when the email address is already used.")]
  public async Task It_should_throw_EmailAddressAlreadyUsedException_when_the_email_address_is_already_used()
  {
    UserAggregate user = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), Faker.Internet.UserName()));
    await _userRepository.SaveAsync(user);

    ReplaceUserPayload payload = new(user.UniqueName.Value)
    {
      Email = new EmailPayload(Faker.Person.Email, isVerified: true)
    };
    ReplaceUserCommand command = new(user.Id.AggregateId.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<EmailAddressAlreadyUsedException>(async () => await Mediator.Send(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.Email.Address, exception.Email.Address);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    UserAggregate user = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), Faker.Internet.UserName()));
    await _userRepository.SaveAsync(user);

    ReplaceUserPayload payload = new(Faker.Person.UserName.ToUpper());
    ReplaceUserCommand command = new(user.Id.AggregateId.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<UserAggregate>>(async () => await Mediator.Send(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.UniqueName.Value);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceUserPayload payload = new("/!\\");
    ReplaceUserCommand command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("UniqueName", exception.Errors.Single().PropertyName);
  }
}
