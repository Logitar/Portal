using Logitar.EventSourcing;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class RealmServiceTests : IntegrationTests, IAsyncLifetime
{
  private readonly IRealmService _realmService;

  private readonly RealmAggregate _realm;

  public RealmServiceTests() : base()
  {
    _realmService = ServiceProvider.GetRequiredService<IRealmService>();

    _realm = new("desjardins", requireConfirmedAccount: true)
    {
      DisplayName = "Desjardins",
      DefaultLocale = new Locale(Faker.Locale),
      Url = new Uri("https://www.desjardins.com/")
    };
    _realm.SetClaimMapping("EmployeeId", new ReadOnlyClaimMapping("employee_no"));
    _realm.SetClaimMapping("HourlyRate", new ReadOnlyClaimMapping("rate", "double"));
    _realm.SetCustomAttribute("PhoneNumber", "+15148454636");
    _realm.SetCustomAttribute("PostalCode", "H2X 3Y2");
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(_realm);
  }

  [Fact(DisplayName = "CreateAsync: it should create a realm.")]
  public async Task CreateAsync_it_should_create_a_realm()
  {
    CreateRealmPayload payload = new()
    {
      UniqueSlug = $"{_realm.UniqueSlug}2",
      DisplayName = "Desjardins",
      Description = "    ",
      DefaultLocale = Faker.Locale,
      Secret = "    ",
      Url = "https://www.desjardins.com/",
      RequireUniqueEmail = false,
      RequireConfirmedAccount = true,
      PasswordSettings = new PasswordSettings
      {
        RequiredLength = 6,
        RequiredUniqueChars = 1,
        RequireNonAlphanumeric = false,
        RequireLowercase = true,
        RequireUppercase = true,
        RequireDigit = true,
        Strategy = "PBKDF2"
      },
      ClaimMappings = new ClaimMapping[]
      {
        new("EmployeeId", "employee_no"),
        new("HourlyRate", "rate", "double")
      },
      CustomAttributes = new CustomAttribute[]
      {
        new("PhoneNumber", "+15148454636"),
        new("PostalCode", "H2X 3Y2")
      }
    };

    Realm realm = await _realmService.CreateAsync(payload);

    Assert.NotEqual(Guid.Empty, realm.Id);
    Assert.Equal(Actor, realm.CreatedBy);
    AssertIsNear(realm.CreatedOn);
    Assert.Equal(Actor, realm.UpdatedBy);
    AssertIsNear(realm.UpdatedOn);
    Assert.True(realm.Version >= 1);

    Assert.Equal(payload.UniqueSlug, realm.UniqueSlug);
    Assert.Equal(payload.DisplayName, realm.DisplayName);
    Assert.Null(realm.Description);
    Assert.Equal(payload.DefaultLocale, realm.DefaultLocale);
    Assert.Equal(32, realm.Secret.Length);
    Assert.Equal(payload.Url, realm.Url);
    Assert.Equal(payload.RequireUniqueEmail, realm.RequireUniqueEmail);
    Assert.Equal(payload.RequireConfirmedAccount, realm.RequireConfirmedAccount);
    Assert.Equal(new ReadOnlyUniqueNameSettings().AllowedCharacters, realm.UniqueNameSettings.AllowedCharacters);
    Assert.Equal(payload.PasswordSettings, realm.PasswordSettings);
    Assert.Equal(payload.ClaimMappings, realm.ClaimMappings);
    Assert.Equal(payload.CustomAttributes, realm.CustomAttributes);
  }

  [Fact(DisplayName = "CreateAsync: it should throw UniqueSlugAlreadyUsedException when the unique slug is already used.")]
  public async Task CreateAsync_it_should_throw_UniqueSlugAlreadyUsedException_when_the_unique_slug_is_already_used()
  {
    CreateRealmPayload payload = new()
    {
      UniqueSlug = _realm.UniqueSlug.ToUpper()
    };

    var exception = await Assert.ThrowsAsync<UniqueSlugAlreadyUsedException>(async () => await _realmService.CreateAsync(payload));
    Assert.Equal(payload.UniqueSlug, exception.UniqueSlug);
    Assert.Equal(nameof(payload.UniqueSlug), exception.PropertyName);
  }

  [Fact(DisplayName = "DeleteAsync: it should delete the realm.")]
  public async Task DeleteAsync_it_should_delete_the_realm()
  {
    UserAggregate user = new(_realm.UniqueNameSettings, Faker.Person.UserName, _realm.Id.Value)
    {
      Email = new EmailAddress(Faker.Person.Email, isVerified: true)
    };
    SessionAggregate session = user.SignIn(_realm.UserSettings);
    await AggregateRepository.SaveAsync(new AggregateRoot[] { user, session });

    Realm? realm = await _realmService.DeleteAsync(_realm.Id.ToGuid());

    Assert.NotNull(realm);
    Assert.Equal(_realm.Id.ToGuid(), realm.Id);

    Assert.Null(await PortalContext.Sessions.SingleOrDefaultAsync(x => x.AggregateId == session.Id.Value));
    Assert.Null(await PortalContext.Users.SingleOrDefaultAsync(x => x.AggregateId == user.Id.Value));
    Assert.Null(await PortalContext.Realms.SingleOrDefaultAsync(x => x.AggregateId == _realm.Id.Value));
  }

  [Fact(DisplayName = "DeleteAsync: it should return null when the realm is not found.")]
  public async Task DeleteAsync_it_should_return_null_when_the_realm_is_not_found()
  {
    Assert.Null(await _realmService.DeleteAsync(Guid.Empty));
  }
}
