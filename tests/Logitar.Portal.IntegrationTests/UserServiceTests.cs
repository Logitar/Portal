using Bogus;
using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class UserServiceTests : IntegrationTests, IAsyncLifetime
{
  private readonly IUserService _userService;

  private readonly RealmAggregate _realm;
  private readonly UserAggregate _user;

  public UserServiceTests() : base()
  {
    _userService = ServiceProvider.GetRequiredService<IUserService>();

    _realm = new("desjardins", requireUniqueEmail: true, requireConfirmedAccount: true)
    {
      DisplayName = "Desjardins",
      DefaultLocale = new Locale(Faker.Locale),
      Url = new Uri("https://www.desjardins.com/")
    };

    _user = new(_realm.UniqueNameSettings, Faker.Person.UserName, _realm.Id.Value)
    {
      Email = new EmailAddress(Faker.Person.Email, isVerified: true),
      FirstName = Faker.Person.FirstName,
      LastName = Faker.Person.LastName,
      Birthdate = Faker.Person.DateOfBirth,
      Gender = new Gender(Faker.Person.Gender.ToString()),
      Locale = new Locale(Faker.Locale),
      TimeZone = new TimeZoneEntry("America/Montreal"),
      Picture = new Uri(Faker.Person.Avatar),
      Website = _realm.Url
    };
    _user.Profile = new Uri($"{_realm.Url}profiles/{_user.Id.ToGuid()}");
    _user.SetPassword(PasswordService.Create(_realm.PasswordSettings, PasswordString));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(new AggregateRoot[] { _realm, _user });
  }

  [Fact(DisplayName = "CreateAsync: it should create a Portal user.")]
  public async Task CreateAsync_it_should_create_a_Portal_user()
  {
    Assert.NotNull(User);
    User.Email = new EmailAddress(Faker.Person.Email);
    await AggregateRepository.SaveAsync(User);

    CreateUserPayload payload = new()
    {
      UniqueName = $" {User.UniqueName}2 ",
      Email = new EmailPayload
      {
        Address = User.Email.Address
      }
    };

    User user = await _userService.CreateAsync(payload);

    Assert.NotEqual(Guid.Empty, user.Id);
    Assert.Equal(Actor, user.CreatedBy);
    AssertIsNear(user.CreatedOn);
    Assert.Equal(Actor, user.UpdatedBy);
    AssertIsNear(user.UpdatedOn);
    Assert.True(user.Version >= 1);

    Assert.Equal(payload.UniqueName.Trim(), user.UniqueName);
    Assert.Equal(payload.Email.Address, user.Email?.Address);

    Assert.Null(user.Realm);
  }

  [Fact(DisplayName = "CreateAsync: it should create a realm user.")]
  public async Task CreateAsync_it_should_create_a_realm_user()
  {
    RealmAggregate realm = new("logitar");
    UserAggregate aggregate = new(realm.UniqueNameSettings, Faker.Person.UserName, realm.Id.Value)
    {
      Email = new EmailAddress(Faker.Person.Email)
    };
    await AggregateRepository.SaveAsync(new AggregateRoot[] { realm, aggregate });

    CreateUserPayload payload = new()
    {
      Realm = realm.Id.ToGuid().ToString(),
      UniqueName = $"{aggregate.UniqueName}2",
      Password = PasswordString,
      IsDisabled = true,
      Address = new AddressPayload
      {
        Street = "150 Saint-Catherine St W",
        Locality = "Montreal",
        Region = "QC",
        PostalCode = "H2X 3Y2",
        Country = "CA"
      },
      Email = new EmailPayload
      {
        Address = aggregate.Email.Address,
        IsVerified = true
      },
      Phone = new PhonePayload
      {
        CountryCode = "CA",
        Number = "+15148454636",
        Extension = "12345"
      },
      FirstName = Faker.Person.FirstName,
      MiddleName = "    ",
      LastName = Faker.Person.LastName,
      Nickname = string.Concat(Faker.Person.FirstName.First(), Faker.Person.LastName).ToLower(),
      Birthdate = Faker.Person.DateOfBirth,
      Gender = Faker.Person.Gender.ToString(),
      Locale = Faker.Locale,
      TimeZone = "  America/Montreal  ",
      Picture = Faker.Person.Avatar,
      Profile = "    ",
      Website = $" https://{Faker.Person.Website}/ ",
      CustomAttributes = new CustomAttribute[]
      {
        new("EmployeeId", "000-068152-0"),
        new("HourlyRate", "38.46")
      }
    };

    User user = await _userService.CreateAsync(payload);

    Assert.NotEqual(Guid.Empty, user.Id);
    Assert.Equal(Actor, user.CreatedBy);
    AssertIsNear(user.CreatedOn);
    Assert.Equal(Actor, user.UpdatedBy);
    AssertIsNear(user.UpdatedOn);
    Assert.True(user.Version >= 1);

    Assert.Equal(payload.UniqueName, user.UniqueName);
    Assert.Equal(payload.IsDisabled, user.IsDisabled);
    AssertEqual(payload.Address, user.Address);
    AssertEqual(payload.Email, user.Email);
    AssertEqual(payload.Phone, user.Phone);
    Assert.Equal(payload.FirstName, user.FirstName);
    Assert.Null(user.MiddleName);
    Assert.Equal(payload.LastName, user.LastName);
    Assert.Equal(payload.Nickname, user.Nickname);
    Assert.Equal(ToUnixTimeMilliseconds(payload.Birthdate), ToUnixTimeMilliseconds(user.Birthdate));
    Assert.Equal(payload.Gender.ToLower(), user.Gender);
    Assert.Equal(payload.Locale, user.Locale);
    Assert.Equal(payload.TimeZone.Trim(), user.TimeZone);
    Assert.Equal(payload.Picture, user.Picture);
    Assert.Null(user.Profile);
    Assert.Equal(payload.Website.Trim(), user.Website);
    Assert.Equal(payload.CustomAttributes, user.CustomAttributes);

    Assert.NotNull(user.Realm);
    Assert.Equal(realm.Id.ToGuid(), user.Realm.Id);

    await AssertUserPasswordAsync(user.Id);
  }

  [Fact(DisplayName = "CreateAsync: it should throw AggregateNotFoundException when the realm is not found.")]
  public async Task CreateAsync_it_should_throw_AggregateNotFoundException_when_the_realm_is_not_found()
  {
    CreateUserPayload payload = new()
    {
      Realm = Guid.Empty.ToString()
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<RealmAggregate>>(async () => await _userService.CreateAsync(payload));
    Assert.Equal(payload.Realm, exception.Id);
    Assert.Equal(nameof(payload.Realm), exception.PropertyName);
  }

  [Fact(DisplayName = "CreateAsync: it should throw EmailAddressAlreadyUsedException when the email address is already used.")]
  public async Task CreateAsync_it_should_throw_EmailAddressAlreadyUsedException_when_the_email_address_is_already_used()
  {
    CreateUserPayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      UniqueName = $"{_user.UniqueName}2",
      Email = new EmailPayload
      {
        Address = Faker.Person.Email
      }
    };

    var exception = await Assert.ThrowsAsync<EmailAddressAlreadyUsedException>(async () => await _userService.CreateAsync(payload));
    Assert.Equal(_realm.Id.Value, exception.TenantId);
    Assert.Equal(payload.Email.Address, exception.EmailAddress);
    Assert.Equal(nameof(payload.Email), exception.PropertyName);
  }

  [Fact(DisplayName = "CreateAsync: it should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task CreateAsync_it_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    CreateUserPayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      UniqueName = _user.UniqueName
    };

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<UserAggregate>>(async () => await _userService.CreateAsync(payload));
    Assert.Equal(_realm.Id.Value, exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "DeleteAsync: it should delete the user.")]
  public async Task DeleteAsync_it_should_delete_the_user()
  {
    SessionAggregate session = new(_user);
    session.SignOut();
    await AggregateRepository.SaveAsync(session);

    User? user = await _userService.DeleteAsync(_user.Id.ToGuid());

    Assert.NotNull(user);
    Assert.Equal(_user.Id.ToGuid(), user.Id);

    Assert.Null(await PortalContext.Sessions.SingleOrDefaultAsync(x => x.AggregateId == session.Id.Value));
    Assert.Null(await PortalContext.Users.SingleOrDefaultAsync(x => x.AggregateId == _user.Id.Value));
  }

  [Fact(DisplayName = "DeleteAsync: it should return null when the user is not found.")]
  public async Task DeleteAsync_it_should_return_null_when_the_user_is_not_found()
  {
    Assert.Null(await _userService.DeleteAsync(Guid.Empty));
  }

  [Fact(DisplayName = "ReadAsync: it should return null when the user is not found.")]
  public async Task ReadAsync_it_should_return_null_when_the_user_is_not_found()
  {
    Assert.Null(await _userService.ReadAsync(Guid.Empty, _realm.UniqueSlug, $"{_user.UniqueName}2"));
  }

  [Fact(DisplayName = "ReadAsync: it should return the Portal user found by unique name.")]
  public async Task ReadAsync_it_should_return_the_Portal_user_found_by_unique_name()
  {
    Assert.NotNull(User);
    User? user = await _userService.ReadAsync(realm: null, uniqueName: $" {User.UniqueName} ");
    Assert.NotNull(user);
    Assert.Equal(User.Id.ToGuid(), user.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should return the realm user found by unique name.")]
  public async Task ReadAsync_it_should_return_the_realm_user_found_by_unique_name()
  {
    User? user = await _userService.ReadAsync(realm: $" {_realm.Id.ToGuid()} ", uniqueName: $" {_user.UniqueName} ");
    Assert.NotNull(user);
    Assert.Equal(_user.Id.ToGuid(), user.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should return the user found by ID.")]
  public async Task ReadAsync_it_should_return_the_user_found_by_Id()
  {
    User? user = await _userService.ReadAsync(_user.Id.ToGuid());
    Assert.NotNull(user);
    Assert.Equal(_user.Id.ToGuid(), user.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should throw TooManyResultsException when many users have been found.")]
  public async Task ReadAsync_it_should_throw_TooManyResultsException_when_many_users_have_been_found()
  {
    Assert.NotNull(User);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<User>>(
      async () => await _userService.ReadAsync(User.Id.ToGuid(), _realm.UniqueSlug, _user.UniqueName)
    );
    Assert.Equal(1, exception.Expected);
    Assert.Equal(2, exception.Actual);
  }

  [Fact(DisplayName = "SearchAsync: it should return empty results when none are matching.")]
  public async Task SearchAsync_it_should_return_empty_results_when_none_are_matching()
  {
    SearchUsersPayload payload = new()
    {
      IdIn = new[] { Guid.Empty }
    };

    SearchResults<User> results = await _userService.SearchAsync(payload);

    Assert.Empty(results.Results);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    UserAggregate ontarian = CreateUser("America/Toronto");

    UserAggregate withPassword = CreateUser();
    withPassword.SetPassword(PasswordService.Create(_realm.PasswordSettings, PasswordString));

    UserAggregate confirmed = CreateUser();
    confirmed.Email = new EmailAddress(Faker.Person.Email, isVerified: true);

    UserAggregate disabled = CreateUser();
    disabled.Disable();

    UserAggregate idNotIn = CreateUser();
    UserAggregate user1 = CreateUser();
    UserAggregate user2 = CreateUser();
    UserAggregate user3 = CreateUser();
    UserAggregate user4 = CreateUser();

    await AggregateRepository.SaveAsync(new AggregateRoot[] { ontarian, withPassword, confirmed, disabled, idNotIn, user1, user2, user3, user4 });

    UserAggregate[] users = new[] { user1, user2, user3, user4 }
      .OrderBy(x => x.FullName).Skip(1).Take(2).ToArray();

    HashSet<Guid> ids = (await PortalContext.Users.AsNoTracking().ToArrayAsync())
      .Select(user => new AggregateId(user.AggregateId).ToGuid()).ToHashSet();
    ids.Remove(idNotIn.Id.ToGuid());

    SearchUsersPayload payload = new()
    {
      Search = new TextSearch
      {
        Operator = SearchOperator.Or,
        Terms = new SearchTerm[]
        {
          new("%/M_ntr__l"),
          new(Guid.NewGuid().ToString())
        }
      },
      IdIn = ids,
      Realm = $" {_realm.UniqueSlug.ToUpper()} ",
      HasPassword = false,
      IsConfirmed = false,
      IsDisabled = false,
      Sort = new UserSortOption[]
      {
        new(UserSort.FullName)
      },
      Skip = 1,
      Limit = 2
    };

    SearchResults<User> results = await _userService.SearchAsync(payload);

    Assert.Equal(users.Length, results.Results.Count());
    Assert.Equal(4, results.Total);

    for (int i = 0; i < users.Length; i++)
    {
      Assert.Equal(users[i].Id.ToGuid(), results.Results.ElementAt(i).Id);
    }
  }
  private UserAggregate CreateUser(string? timeZone = null)
  {
    Faker faker = new();

    return new UserAggregate(_realm.UniqueNameSettings, faker.Person.UserName, _realm.Id.Value)
    {
      FirstName = faker.Person.FirstName,
      LastName = faker.Person.LastName,
      Nickname = string.Concat(faker.Person.FirstName.First(), faker.Person.LastName).ToLower(),
      Birthdate = faker.Person.DateOfBirth,
      Gender = new Gender(faker.Person.Gender.ToString()),
      Locale = new Locale(faker.Locale),
      TimeZone = new TimeZoneEntry(timeZone ?? "America/Montreal"),
      Picture = new Uri(faker.Person.Avatar),
      Website = new Uri($"https://www.{faker.Person.Website}/")
    };
  }

  [Fact(DisplayName = "SignOutAsync: it should sign-out the user.")]
  public async Task SignOutAsync_it_should_sign_out_the_user()
  {
    SessionAggregate session1 = new(_user);
    SessionAggregate session2 = new(_user);
    SessionAggregate signedOut = new(_user);
    signedOut.SignOut();
    await AggregateRepository.SaveAsync(new[] { session1, session2, signedOut });

    User? user = await _userService.SignOutAsync(_user.Id.ToGuid());

    Assert.NotNull(user);
    Assert.Equal(_user.Id.ToGuid(), user.Id);

    SessionEntity[] entities = await PortalContext.Sessions.AsNoTracking()
      .Include(x => x.User)
      .Where(x => x.User!.AggregateId == _user.Id.Value)
      .ToArrayAsync();

    Assert.Equal(3, entities.Length);
    Assert.Contains(entities, session => session.AggregateId == session1.Id.Value && session.IsActive == false
      /*&& session.SignedOutBy == ActorId.Value*/ && (DateTime.Now - session.SignedOutOn) < TimeSpan.FromMinutes(1));
    Assert.Contains(entities, session => session.AggregateId == session2.Id.Value && session.IsActive == false
      /*&& session.SignedOutBy == ActorId.Value*/&& (DateTime.Now - session.SignedOutOn) < TimeSpan.FromMinutes(1));
    Assert.Contains(entities, session => session.AggregateId == signedOut.Id.Value && session.IsActive == false
      && session.SignedOutBy == signedOut.UpdatedBy.Value
      && ToUnixTimeMilliseconds(session.SignedOutOn) == ToUnixTimeMilliseconds(signedOut.UpdatedOn));
  }

  [Fact(DisplayName = "SignOutAsync: it should return null when the user is not found.")]
  public async Task SignOutAsync_it_should_return_null_when_the_user_is_not_found()
  {
    Assert.Null(await _userService.SignOutAsync(Guid.Empty));
  }

  private static void AssertEqual(AddressPayload? payload, Address? address)
  {
    if (payload == null || address == null)
    {
      Assert.Null(payload);
      Assert.Null(address);
    }
    else
    {
      Assert.Equal(payload.Street, address.Street);
      Assert.Equal(payload.Locality, address.Locality);
      Assert.Equal(payload.Region, address.Region);
      Assert.Equal(payload.PostalCode, address.PostalCode);
      Assert.Equal(payload.Country, address.Country);
      Assert.Equal(payload.IsVerified, address.IsVerified);
    }
  }
  private static void AssertEqual(EmailPayload? payload, Email? email)
  {
    if (payload == null || email == null)
    {
      Assert.Null(payload);
      Assert.Null(email);
    }
    else
    {
      Assert.Equal(payload.Address, email.Address);
      Assert.Equal(payload.IsVerified, email.IsVerified);
    }
  }
  private static void AssertEqual(PhonePayload? payload, Phone? phone)
  {
    if (payload == null || phone == null)
    {
      Assert.Null(payload);
      Assert.Null(phone);
    }
    else
    {
      Assert.Equal(payload.CountryCode, phone.CountryCode);
      Assert.Equal(payload.Number, phone.Number);
      Assert.Equal(payload.Extension, phone.Extension);
      Assert.Equal(payload.IsVerified, phone.IsVerified);
    }
  }

  private static long? ToUnixTimeMilliseconds(DateTime? value)
    => value.HasValue ? new DateTimeOffset(value.Value).ToUnixTimeMilliseconds() : null;
}
