using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Realms;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Services;

[Trait(Traits.Category, Categories.Integration)]
public class UserServiceTests : IntegrationTestBase, IAsyncLifetime
{
  private readonly IRealmRepository _realmRepository;
  private readonly IUserManager _userManager;
  private readonly IUserRepository _userRepository;
  private readonly IUserService _userService;

  private readonly RealmAggregate _realm;
  private readonly UserAggregate _user;

  public UserServiceTests() : base()
  {
    _realmRepository = ServiceProvider.GetRequiredService<IRealmRepository>();
    _userManager = ServiceProvider.GetRequiredService<IUserManager>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
    _userService = ServiceProvider.GetRequiredService<IUserService>();

    _realm = new("desjardins", ActorId)
    {
      DisplayName = "Desjardins",
      DefaultLocale = CultureInfo.GetCultureInfo(Faker.Locale),
      Url = new Uri("https://www.desjardins.com/"),
      RequireUniqueEmail = true,
      RequireConfirmedAccount = true
    };
    _realm.SetClaimMapping("EmployeeId", new ReadOnlyClaimMapping("employee_no", "UInt32"));

    _user = new(_realm.UniqueNameSettings, Faker.Person.UserName, _realm.Id.Value, ActorId)
    {
      Email = new EmailAddress(Faker.Person.Email)
    };
  }

  [Fact(DisplayName = "CreateAsync: it should create the user.")]
  public async Task CreateAsync_it_should_create_the_user()
  {
    CreateUserPayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      UniqueName = $" {Faker.Person.UserName}2 ",
      Password = "Pizza7",
      IsDisabled = true,
      Email = new EmailPayload
      {
        Address = $" {Faker.Person.Email} "
      },
      Phone = new PhonePayload
      {
        CountryCode = " CA ",
        Number = " +18668667000 ",
        Extension = " 12345 ",
        IsVerified = true
      },
      FirstName = $" {Faker.Person.FirstName} ",
      MiddleName = "  ",
      LastName = $" {Faker.Person.LastName} ",
      Nickname = $" {Faker.Person.UserName} ",
      Birthdate = Faker.Person.DateOfBirth,
      Gender = $" {Faker.Person.Gender} ",
      Locale = $" {Faker.Locale} ",
      TimeZone = " America/Montreal ",
      Picture = $" {Faker.Person.Avatar} ",
      Profile = "  ",
      Website = " https://www.desjardins.com/ ",
      CustomAttributes = new CustomAttribute[]
      {
        new("EmployeeId", "001-670671-9"),
        new("SocialSecurityNumber", "557-509-780")
      }
    };

    User user = await _userService.CreateAsync(payload);
    Assert.NotNull(user);
    Assert.Equal(new Actor()/*Actor*/, user.CreatedBy); // TODO(fpion): resolve actor
    Assert.Equal(new Actor()/*Actor*/, user.UpdatedBy); // TODO(fpion): resolve actor
    Assert.NotNull(user.Realm);
    Assert.Equal(_realm.Id.Value, user.Realm.Id);
    Assert.Equal(payload.UniqueName.Trim(), user.UniqueName);
    Assert.True(user.HasPassword);
    Assert.Null(user.PasswordChangedBy); // TODO(fpion): resolve actor
    Assert.NotNull(user.PasswordChangedOn);
    Assert.Null(user.DisabledBy); // TODO(fpion): resolve actor
    Assert.NotNull(user.DisabledOn);
    Assert.True(user.IsDisabled);
    Assert.Null(user.AuthenticatedOn);
    Assert.Null(user.Address);
    Assert.NotNull(user.Email);
    Assert.Equal(payload.Email.Address.Trim(), user.Email.Address);
    Assert.Null(user.Email.VerifiedBy);
    Assert.Null(user.Email.VerifiedOn);
    Assert.False(user.Email.IsVerified);
    Assert.NotNull(user.Phone);
    Assert.Equal(payload.Phone.CountryCode.Trim(), user.Phone.CountryCode);
    Assert.Equal(payload.Phone.Number.Trim(), user.Phone.Number);
    Assert.Equal(payload.Phone.Extension.Trim(), user.Phone.Extension);
    Assert.Null(user.Phone.VerifiedBy); // TODO(fpion): resolve actor
    Assert.NotNull(user.Phone.VerifiedOn);
    Assert.True(user.Phone.IsVerified);
    Assert.True(user.IsConfirmed);
    Assert.Equal(payload.FirstName.CleanTrim(), user.FirstName);
    Assert.Equal(payload.MiddleName.CleanTrim(), user.MiddleName);
    Assert.Equal(payload.LastName.CleanTrim(), user.LastName);
    Assert.NotNull(user.FullName);
    Assert.Equal(payload.Nickname.CleanTrim(), user.Nickname);
    Assert.Equal(payload.Birthdate?.ToUniversalTime(), user.Birthdate);
    Assert.Equal(payload.Gender.ToLower().CleanTrim(), user.Gender);
    Assert.Equal(payload.Locale.CleanTrim(), user.Locale);
    Assert.Equal(payload.TimeZone.CleanTrim(), user.TimeZone);
    Assert.Equal(payload.Picture.CleanTrim(), user.Picture);
    Assert.Equal(payload.Profile.CleanTrim(), user.Profile);
    Assert.Equal(payload.Website.CleanTrim(), user.Website);
    Assert.Equal(payload.CustomAttributes, user.CustomAttributes);
  }

  [Fact(DisplayName = "CreateAsync: it should throw AggregateNotFoundException when realm could not be found.")]
  public async Task CreateAsync_it_should_throw_AggregateNotFoundException_when_realm_could_not_be_found()
  {
    CreateUserPayload payload = new()
    {
      Realm = Guid.Empty.ToString()
    };
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<RealmAggregate>>(
      async () => await _userService.CreateAsync(payload)
    );
    Assert.Equal(payload.Realm, exception.Id);
    Assert.Equal(nameof(payload.Realm), exception.PropertyName);
  }

  [Fact(DisplayName = "CreateAsync: it should throw EmailAddressAlreadyUsedException when unique name is already used.")]
  public Task CreateAsync_it_should_throw_EmailAddressAlreadyUsedException_when_unique_name_is_already_used()
  {
    return Task.CompletedTask; // TODO(fpion): IOptions<UserSettings> are Scoped/Singleton instead of Transient!

    //Assert.NotNull(_user.Email);

    //CreateUserPayload payload = new()
    //{
    //  Realm = _realm.Id.Value,
    //  UniqueName = $" {_user.UniqueName}2 ",
    //  Email = new EmailPayload
    //  {
    //    Address = _user.Email.Address
    //  }
    //};
    //var exception = await Assert.ThrowsAsync<EmailAddressAlreadyUsedException>(
    //  async () => await _userService.CreateAsync(payload)
    //);
    //Assert.Equal(_realm.Id.Value, exception.TenantId);
    //Assert.Equal(payload.Email.Address, exception.EmailAddress);
    //Assert.Equal(nameof(payload.Realm), exception.PropertyName);
  }

  [Fact(DisplayName = "CreateAsync: it should throw UniqueNameAlreadyUsedException when unique name is already used.")]
  public async Task CreateAsync_it_should_throw_UniqueNameAlreadyUsedException_when_unique_name_is_already_used()
  {
    CreateUserPayload payload = new()
    {
      Realm = _realm.Id.Value,
      UniqueName = $" {_user.UniqueName} "
    };
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<UserAggregate>>(
      async () => await _userService.CreateAsync(payload)
    );
    Assert.Equal(_realm.Id.Value, exception.TenantId);
    Assert.Equal(payload.UniqueName.Trim(), exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "DeleteAsync: it should delete the user.")]
  public async Task DeleteAsync_it_should_delete_the_user()
  {
    User? user = await _userService.DeleteAsync(_user.Id.Value);
    Assert.NotNull(user);
    Assert.Equal(_user.Id.Value, user.Id);

    Assert.Null(await _userRepository.LoadAsync(_user.Id));
    Assert.Empty(IdentityContext.Users.Where(x => x.AggregateId == _user.Id.Value));
  }

  [Fact(DisplayName = "DeleteAsync: it should return return null when user is not found.")]
  public async Task DeleteAsync_it_should_return_return_null_when_user_is_not_found()
  {
    Assert.Null(await _userService.DeleteAsync(Guid.Empty.ToString()));
  }

  [Fact(DisplayName = "ReadAsync: it should read the user.")]
  public async Task ReadAsync_it_should_read_the_user()
  {
    User? user = await _userService.ReadAsync(_user.Id.Value, _user.TenantId, _user.UniqueName);
    Assert.NotNull(user);
    Assert.Equal(_user.Id.Value, user.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should return null when user is not found.")]
  public async Task ReadAsync_it_should_return_null_when_user_is_not_found()
  {
    Assert.Null(await _userService.ReadAsync(Guid.Empty.ToString(), $"{_user.UniqueName}2"));
  }

  [Fact(DisplayName = "ReadAsync: it should throw TooManyResultsException when multiple users are found.")]
  public async Task ReadAsync_it_should_throw_TooManyResultsException_when_multiple_users_are_found()
  {
    UserAggregate user = new(_realm.UniqueNameSettings, $"{_user.UniqueName}2", _realm.Id.Value, ActorId);
    await _userManager.SaveAsync(user);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<User>>(
      async () => await _userService.ReadAsync(_user.Id.Value, user.TenantId, user.UniqueName)
    );
    Assert.Equal(1, exception.Expected);
    Assert.Equal(2, exception.Actual);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _realmRepository.SaveAsync(_realm);
    await _userManager.SaveAsync(_user);
  }
}
