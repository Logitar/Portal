using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
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

    _realm = new("desjardins", requireUniqueEmail: true, requireConfirmedAccount: true, actorId: ActorId)
    {
      DisplayName = "Desjardins",
      DefaultLocale = new Locale(Faker.Locale),
      Url = new Uri("https://www.desjardins.com/")
    };
    _realm.SetClaimMapping("EmployeeId", new ReadOnlyClaimMapping("employee_no"));
    _realm.SetClaimMapping("HourlyRate", new ReadOnlyClaimMapping("flat_rate", "double"));
    _realm.SetCustomAttribute("AdministrativeRegion", "Montréal");
    _realm.SetCustomAttribute("RegionalBranch", "946");
    _realm.Update(ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(new[] { _realm });
  }

  [Fact(DisplayName = "CreateAsync: it should create a new realm.")]
  public async Task CreateAsync_it_should_create_a_new_realm()
  {
    CreateRealmPayload payload = new()
    {
      UniqueSlug = $" {_realm.UniqueSlug}2 ",
      DisplayName = " Desjardins ",
      Description = "  ",
      DefaultLocale = $" {Faker.Locale} ",
      Secret = "  ",
      Url = " https://www.desjardins.com/ ",
      RequireUniqueEmail = true,
      RequireConfirmedAccount = true,
      PasswordSettings = new()
      {
        RequiredLength = 7,
        RequiredUniqueChars = 4,
        RequireNonAlphanumeric = true,
        RequireLowercase = true,
        RequireUppercase = true,
        RequireDigit = true
      },
      ClaimMappings = new ClaimMapping[]
      {
        new(" EmployeeId ", " employee_no ", string.Empty),
        new("HourlyRate", "flat_rate", "double")
      },
      CustomAttributes = new CustomAttribute[]
      {
        new("AdministrativeRegion", "Montréal"),
        new(" RegionalBranch ", " 946 ")
      }
    };

    Realm realm = await _realmService.CreateAsync(payload);

    Assert.Equal(Actor, realm.CreatedBy);
    AssertIsNear(realm.CreatedOn);
    Assert.Equal(Actor, realm.UpdatedBy);
    AssertIsNear(realm.UpdatedOn);
    Assert.True(realm.Version >= 1);

    Assert.Equal(payload.UniqueSlug.Trim(), realm.UniqueSlug);
    Assert.Equal(payload.DisplayName.Trim(), realm.DisplayName);
    Assert.Equal(payload.Description?.CleanTrim(), realm.Description);
    Assert.Equal(payload.DefaultLocale.Trim(), realm.DefaultLocale);
    Assert.Equal(32, realm.Secret.Length);
    Assert.Equal(payload.Url.Trim(), realm.Url);
    Assert.Equal(payload.RequireUniqueEmail, realm.RequireUniqueEmail);
    Assert.Equal(payload.RequireConfirmedAccount, realm.RequireConfirmedAccount);
    Assert.Equal("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+", realm.UniqueNameSettings.AllowedCharacters);
    Assert.Equal(payload.PasswordSettings, realm.PasswordSettings);

    Assert.Equal(2, realm.ClaimMappings.Count());
    Assert.Contains(realm.ClaimMappings, claimMapping => claimMapping.Key == "EmployeeId"
      && claimMapping.Name == "employee_no" && claimMapping.Type == null);
    Assert.Contains(realm.ClaimMappings, claimMapping => claimMapping.Key == "HourlyRate"
      && claimMapping.Name == "flat_rate" && claimMapping.Type == "double");

    Assert.Equal(2, realm.CustomAttributes.Count());
    Assert.Contains(realm.CustomAttributes, customAttribute => customAttribute.Key == "AdministrativeRegion"
      && customAttribute.Value == "Montréal");
    Assert.Contains(realm.CustomAttributes, customAttribute => customAttribute.Key == "RegionalBranch"
      && customAttribute.Value == "946");
  }

  [Fact(DisplayName = "CreateAsync: it should throw UniqueSlugAlreadyUsedException when the unique slug is already used.")]
  public async Task CreateAsync_it_should_throw_UniqueSlugAlreadyUsedException_when_the_unique_slug_is_already_used()
  {
    CreateRealmPayload payload = new()
    {
      UniqueSlug = _realm.UniqueSlug
    };

    var exception = await Assert.ThrowsAsync<UniqueSlugAlreadyUsedException>(
      async () => await _realmService.CreateAsync(payload)
    );
    Assert.Equal(payload.UniqueSlug, exception.UniqueSlug);
    Assert.Equal(nameof(payload.UniqueSlug), exception.PropertyName);
  }

  [Fact(DisplayName = "DeleteAsync: it should return null when the realm is not found.")]
  public async Task DeleteAsync_it_should_return_null_when_the_realm_is_not_found()
  {
    Assert.Null(await _realmService.DeleteAsync(Guid.Empty.ToString()));
  }

  [Fact(DisplayName = "DeleteAsync: it should return the deleted realm.")]
  public async Task DeleteAsync_it_should_return_the_deleted_realm()
  {
    UserAggregate user = new(_realm.UniqueNameSettings, Faker.Person.UserName, _realm.Id.Value, ActorId)
    {
      Email = new EmailAddress(Faker.Person.Email, isVerified: true)
    };
    SessionAggregate session = user.SignIn(_realm.UserSettings, password: null, secret: null); // TODO(fpion): simplify method
    await AggregateRepository.SaveAsync(new AggregateRoot[] { user, session });

    Realm? realm = await _realmService.DeleteAsync(_realm.Id.Value);
    Assert.NotNull(realm);
    Assert.Equal(_realm.Id.Value, realm.Id);

    RealmEntity? realmEntity = await PortalContext.Realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == _realm.Id.Value);
    Assert.Null(realmEntity);

    EventEntity? deleted = await EventContext.Events.AsNoTracking()
      .SingleOrDefaultAsync(e => e.AggregateType == typeof(RealmAggregate).GetName()
        && e.AggregateId == _realm.Id.Value && e.EventType == typeof(RealmDeletedEvent).GetName());
    Assert.NotNull(deleted);
    Assert.Equal(Actor.Id, deleted.ActorId);
    AssertIsNear(deleted.OccurredOn);
  }

  [Fact(DisplayName = "ReadAsync: it should return null when the realm is not found.")]
  public async Task ReadAsync_it_should_return_null_when_the_realm_is_not_found()
  {
    Assert.Null(await _realmService.ReadAsync(Guid.Empty.ToString(), $"{_realm.UniqueSlug}2"));
  }

  [Fact(DisplayName = "ReadAsync: it should return the realm found by ID.")]
  public async Task ReadAsync_it_should_return_the_realm_found_by_id()
  {
    Realm? realm = await _realmService.ReadAsync(_realm.Id.Value);
    Assert.NotNull(realm);
    Assert.Equal(_realm.Id.Value, realm.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should return the realm found by unique slug.")]
  public async Task ReadAsync_it_should_return_the_realm_found_by_unique_slug()
  {
    Realm? realm = await _realmService.ReadAsync(uniqueSlug: _realm.UniqueSlug);
    Assert.NotNull(realm);
    Assert.Equal(_realm.Id.Value, realm.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should throw TooManyResultsException when there are too many results.")]
  public async Task ReadAsync_it_should_throw_TooManyResultsException_when_there_are_too_many_results()
  {
    RealmAggregate realm = new($"{_realm.UniqueSlug}2");
    await AggregateRepository.SaveAsync(realm);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<Realm>>(
      async () => await _realmService.ReadAsync(_realm.Id.Value, realm.UniqueSlug)
    );
    Assert.Equal(1, exception.Expected);
    Assert.Equal(2, exception.Actual);
  }

  [Fact(DisplayName = "SearchAsync: it should return no result when there are no match.")]
  public async Task SearchAsync_it_should_return_no_result_when_there_are_no_match()
  {
    SearchRealmsPayload payload = new()
    {
      Search = new TextSearch
      {
        Terms = new SearchTerm[]
        {
          new("test")
        }
      }
    };

    SearchResults<Realm> results = await _realmService.SearchAsync(payload);

    Assert.Empty(results.Results);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results by ID.")]
  public async Task SearchAsync_it_should_return_the_correct_results_by_ID()
  {
    RealmAggregate realm1 = new("realm1");
    RealmAggregate realm2 = new("realm2");
    await AggregateRepository.SaveAsync(new[] { realm1, realm2 });

    SearchRealmsPayload payload = new()
    {
      Id = new TextSearch
      {
        Operator = QueryOperator.Or,
        Terms = new SearchTerm[]
        {
          new(_realm.Id.Value),
          new(realm1.Id.Value),
          new(Guid.Empty.ToString())
        }
      }
    };

    SearchResults<Realm> results = await _realmService.SearchAsync(payload);

    Assert.Equal(2, results.Results.Count());
    Assert.Equal(2, results.Total);

    Assert.Contains(results.Results, realm => realm.Id == _realm.Id.Value);
    Assert.Contains(results.Results, realm => realm.Id == realm1.Id.Value);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    RealmAggregate realm1 = new("test");
    RealmAggregate realm2 = new("delafleur");
    RealmAggregate realm3 = new("leurre");
    RealmAggregate realm4 = new("beurre");
    await AggregateRepository.SaveAsync(new[] { realm1, realm2, realm3, realm4 });

    RealmAggregate[] realms = new[] { realm1, realm2, realm3, realm4 }
      .OrderBy(r => r.DisplayName)
      .Skip(1).Take(2)
      .ToArray();

    SearchRealmsPayload payload = new()
    {
      Search = new TextSearch
      {
        Operator = QueryOperator.Or,
        Terms = new SearchTerm[]
        {
          new("%e__"),
          new("%eur%")
        }
      },
      Sort = new RealmSortOption[]
      {
        new(RealmSort.DisplayName)
      },
      Skip = 1,
      Limit = 2
    };

    SearchResults<Realm> results = await _realmService.SearchAsync(payload);

    Assert.Equal(realms.Length, results.Results.Count());
    Assert.Equal(4, results.Total);

    for (int i = 0; i < realms.Length; i++)
    {
      Assert.Equal(realms[i].Id.Value, results.Results.ElementAt(i).Id);
    }
  }

  [Fact(DisplayName = "UpdateAsync: it should return null when the realm is not found.")]
  public async Task UpdateAsync_it_should_return_null_when_the_realm_is_not_found()
  {
    UpdateRealmPayload payload = new();
    Assert.Null(await _realmService.UpdateAsync(Guid.Empty.ToString(), payload));
  }

  [Fact(DisplayName = "UpdateAsync: it should throw UniqueSlugAlreadyUsedException when the unique slug is already used.")]
  public async Task UpdateAsync_it_should_throw_UniqueSlugAlreadyUsedException_when_the_unique_slug_is_already_used()
  {
    CreateRealmPayload createPayload = new()
    {
      UniqueSlug = $" {_realm.UniqueSlug}2 "
    };
    Realm realm = await _realmService.CreateAsync(createPayload);

    UpdateRealmPayload updatePayload = new()
    {
      UniqueSlug = $" {_realm.UniqueSlug} "
    };
    var exception = await Assert.ThrowsAsync<UniqueSlugAlreadyUsedException>(
      async () => await _realmService.UpdateAsync(realm.Id, updatePayload)
    );
    Assert.Equal(updatePayload.UniqueSlug, exception.UniqueSlug);
    Assert.Equal(nameof(updatePayload.UniqueSlug), exception.PropertyName);
  }

  [Fact(DisplayName = "UpdateAsync: it should update the existing realm.")]
  public async Task UpdateAsync_it_should_update_the_existing_realm()
  {
    string oldSecret = _realm.Secret;

    UpdateRealmPayload payload = new()
    {
      UniqueSlug = " desjardins-new ",
      Description = new Modification<string>(" This is the new realm for the Desjardins Montréal branch. "),
      Secret = string.Empty,
      PasswordSettings = new PasswordSettings
      {
        RequiredLength = 8,
        RequiredUniqueChars = 8,
        RequireNonAlphanumeric = true,
        RequireLowercase = true,
        RequireUppercase = true,
        RequireDigit = true
      },
      ClaimMappings = new ClaimMappingModification[]
      {
        new(" Department ", " department "),
        new("EmployeeId", "employee_no", "string"),
        new("HourlyRate", name: null)
      },
      CustomAttributes = new CustomAttributeModification[]
      {
        new(" RegionalCode ", " 06 "),
        new("RegionalBranch", "951"),
        new("AdministrativeRegion", value: null)
      }
    };

    Realm? realm = await _realmService.UpdateAsync(_realm.Id.Value, payload);

    Assert.NotNull(realm);
    Assert.Equal(_realm.Id.Value, realm.Id);
    Assert.Equal(Actor, realm.UpdatedBy);
    AssertIsNear(realm.UpdatedOn);
    Assert.True(realm.Version > 1);

    Assert.Equal(payload.UniqueSlug.Trim(), realm.UniqueSlug);
    Assert.Equal(payload.Description.Value?.CleanTrim(), realm.Description);
    Assert.NotEqual(oldSecret, realm.Secret);
    Assert.Equal("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+", realm.UniqueNameSettings.AllowedCharacters);
    Assert.Equal(payload.PasswordSettings, realm.PasswordSettings);

    Assert.Equal(2, realm.ClaimMappings.Count());
    Assert.Contains(realm.ClaimMappings, claimMapping => claimMapping.Key == "Department"
      && claimMapping.Name == "department" && claimMapping.Type == null);
    Assert.Contains(realm.ClaimMappings, claimMapping => claimMapping.Key == "EmployeeId"
      && claimMapping.Name == "employee_no" && claimMapping.Type == "string");

    Assert.Equal(2, realm.CustomAttributes.Count());
    Assert.Contains(realm.CustomAttributes, customAttribute => customAttribute.Key == "RegionalCode" && customAttribute.Value == "06");
    Assert.Contains(realm.CustomAttributes, customAttribute => customAttribute.Key == "RegionalBranch" && customAttribute.Value == "951");
  }
}
