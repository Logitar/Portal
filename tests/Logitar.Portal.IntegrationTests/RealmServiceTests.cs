using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class RealmServiceTests : IntegrationTests, IAsyncLifetime
{
  private readonly IRealmService _realmService;

  private readonly RealmAggregate _realm;
  private readonly SenderAggregate _sender;
  private readonly TemplateAggregate _template;

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
    string tenantId = _realm.Id.Value;

    _sender = new(Faker.Internet.Email(), ProviderType.SendGrid, isDefault: true, tenantId)
    {
      DisplayName = Faker.Name.FullName()
    };

    _template = new(_realm.UniqueNameSettings, "PasswordRecovery", "PasswordRecovery_Subject", MediaTypeNames.Text.Plain, "PasswordRecovery_Body", tenantId)
    {
      DisplayName = "Password Recovery"
    };
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(new AggregateRoot[] { _realm, _sender, _template });
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
    string tenantId = _realm.Id.Value;

    SenderAggregate sender = new(Faker.Internet.Email(), ProviderType.SendGrid, isDefault: false, tenantId);
    _realm.SetPasswordRecoverySender(sender);

    TemplateAggregate template = new(_realm.UniqueNameSettings, "CreateUser", "CreateUser_Subject", MediaTypeNames.Text.Plain, "CreateUser_Body", tenantId);
    _realm.SetPasswordRecoveryTemplate(template);

    Recipients recipients = new(new ReadOnlyRecipient[]
    {
      new(Faker.Person.Email, Faker.Person.FullName)
    });
    MessageAggregate message = new("Test", "Hello World!", recipients, sender, template, _realm);

    DictionaryAggregate dictionary = new(new Locale("fr"), tenantId);

    RoleAggregate role = new(_realm.UniqueNameSettings, "admin", tenantId);

    Password secret = PasswordService.Generate(_realm.PasswordSettings, ApiKeyAggregate.SecretLength, out _);
    ApiKeyAggregate apiKey = new("Default", secret, tenantId);
    apiKey.AddRole(role);

    UserAggregate user = new(_realm.UniqueNameSettings, Faker.Person.UserName, tenantId);
    user.AddRole(role);

    SessionAggregate session = new(user);

    await AggregateRepository.SaveAsync(new AggregateRoot[] { sender, template, message, _realm, dictionary, role, apiKey, user, session });

    Realm? realm = await _realmService.DeleteAsync(_realm.Id.ToGuid());

    Assert.NotNull(realm);
    Assert.Equal(_realm.Id.ToGuid(), realm.Id);

    Assert.Null(await PortalContext.Messages.SingleOrDefaultAsync(x => x.AggregateId == message.Id.Value));
    Assert.Null(await PortalContext.Templates.SingleOrDefaultAsync(x => x.AggregateId == template.Id.Value));
    Assert.Null(await PortalContext.Senders.SingleOrDefaultAsync(x => x.AggregateId == sender.Id.Value));
    Assert.Null(await PortalContext.Dictionaries.SingleOrDefaultAsync(x => x.AggregateId == dictionary.Id.Value));
    Assert.Null(await PortalContext.Sessions.SingleOrDefaultAsync(x => x.AggregateId == session.Id.Value));
    Assert.Null(await PortalContext.Users.SingleOrDefaultAsync(x => x.AggregateId == user.Id.Value));
    Assert.Null(await PortalContext.ApiKeys.SingleOrDefaultAsync(x => x.AggregateId == apiKey.Id.Value));
    Assert.Null(await PortalContext.Roles.SingleOrDefaultAsync(x => x.AggregateId == role.Id.Value));
    Assert.Null(await PortalContext.Realms.SingleOrDefaultAsync(x => x.AggregateId == _realm.Id.Value));
  }

  [Fact(DisplayName = "DeleteAsync: it should return null when the realm is not found.")]
  public async Task DeleteAsync_it_should_return_null_when_the_realm_is_not_found()
  {
    Assert.Null(await _realmService.DeleteAsync(Guid.Empty));
  }

  [Fact(DisplayName = "ReadAsync: it should return null when the realm is not found.")]
  public async Task ReadAsync_it_should_return_null_when_the_realm_is_not_found()
  {
    Assert.Null(await _realmService.ReadAsync(Guid.Empty, $"{_realm.UniqueSlug}-2"));
  }

  [Fact(DisplayName = "ReadAsync: it should return the realm found by ID.")]
  public async Task ReadAsync_it_should_return_the_realm_found_by_Id()
  {
    Realm? realm = await _realmService.ReadAsync(_realm.Id.ToGuid());
    Assert.NotNull(realm);
    Assert.Equal(_realm.Id.ToGuid(), realm.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should return the realm found by unique slug.")]
  public async Task ReadAsync_it_should_return_the_realmfound_by_unique_slug()
  {
    Realm? realm = await _realmService.ReadAsync(uniqueSlug: $" {_realm.UniqueSlug} ");
    Assert.NotNull(realm);
    Assert.Equal(_realm.Id.ToGuid(), realm.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should throw TooManyResultsException when many realms have been found.")]
  public async Task ReadAsync_it_should_throw_TooManyResultsException_when_many_realms_have_been_found()
  {
    RealmAggregate realm = new($"{_realm.UniqueSlug}-2");
    await AggregateRepository.SaveAsync(realm);

    Assert.NotNull(User);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<Realm>>(
      async () => await _realmService.ReadAsync(_realm.Id.ToGuid(), realm.UniqueSlug)
    );
    Assert.Equal(1, exception.Expected);
    Assert.Equal(2, exception.Actual);
  }

  [Fact(DisplayName = "ReplaceAsync: it should replace the realm.")]
  public async Task ReplaceAsync_it_should_replace_the_realm()
  {
    long version = _realm.Version;

    _realm.RemoveClaimMapping("EmployeeId");
    _realm.SetClaimMapping("EmployeeNo", new ReadOnlyClaimMapping("employee_no"));
    _realm.RemoveCustomAttribute("PostalCode");
    _realm.SetCustomAttribute("PostalAddress", "150 Saint-Catherine St W, Montreal, Quebec H2X 3Y2");
    await AggregateRepository.SaveAsync(_realm);

    ReplaceRealmPayload payload = new()
    {
      UniqueSlug = $" {_realm.UniqueSlug}-2 ",
      DisplayName = $"  {_realm.DisplayName?.ToUpper()}  ",
      Description = "    ",
      DefaultLocale = "  en-CA  ",
      Secret = "  YRJH43_+dM}Z&a8K!P5c,NsSvtebn2rT  ",
      Url = "  https://www.desjardins.com/index.html  ",
      RequireUniqueEmail = true,
      RequireConfirmedAccount = true,
      UniqueNameSettings = new UniqueNameSettings
      {
        AllowedCharacters = _realm.UniqueNameSettings.AllowedCharacters
      },
      PasswordSettings = new PasswordSettings
      {
        RequiredLength = 7,
        RequiredUniqueChars = 4,
        RequireNonAlphanumeric = true,
        RequireLowercase = true,
        RequireUppercase = true,
        RequireDigit = false,
        Strategy = "BCRYPT"
      },
      ClaimMappings = new ClaimMapping[]
      {
        new("DepartmentCode", "dept_code", "string"),
        new("HourlyRate", "rate", "float")
      },
      CustomAttributes = new CustomAttribute[]
      {
        new("LocationKind", "ShoppingMall"),
        new("PhoneNumber", "+15148454636")
      },
      PasswordRecoverySenderId = _sender.Id.ToGuid(),
      PasswordRecoveryTemplateId = _template.Id.ToGuid()
    };

    Realm? realm = await _realmService.ReplaceAsync(_realm.Id.ToGuid(), payload, version);
    Assert.NotNull(realm);

    Assert.Equal(_realm.Id.ToGuid(), realm.Id);
    Assert.Equal(Guid.Empty, realm.CreatedBy.Id);
    AssertEqual(_realm.CreatedOn, realm.CreatedOn);
    Assert.Equal(Actor, realm.UpdatedBy);
    AssertIsNear(realm.UpdatedOn);
    Assert.True(realm.Version > version);

    Assert.Equal(payload.UniqueSlug.Trim(), realm.UniqueSlug);
    Assert.Equal(payload.DisplayName.Trim(), realm.DisplayName);
    Assert.Null(realm.Description);
    Assert.Equal(payload.DefaultLocale.Trim(), realm.DefaultLocale);
    Assert.Equal(payload.Secret.Trim(), realm.Secret);
    Assert.Equal(payload.Url.Trim(), realm.Url);
    Assert.Equal(payload.RequireUniqueEmail, realm.RequireUniqueEmail);
    Assert.Equal(payload.RequireConfirmedAccount, realm.RequireConfirmedAccount);
    Assert.Equal(payload.UniqueNameSettings, realm.UniqueNameSettings);
    Assert.Equal(payload.PasswordSettings, realm.PasswordSettings);

    Assert.Equal(3, realm.ClaimMappings.Count());
    Assert.Contains(realm.ClaimMappings, claimMapping => claimMapping.Key == "DepartmentCode"
      && claimMapping.Name == "dept_code" && claimMapping.Type == "string");
    Assert.Contains(realm.ClaimMappings, claimMapping => claimMapping.Key == "EmployeeNo"
      && claimMapping.Name == "employee_no" && claimMapping.Type == null);
    Assert.Contains(realm.ClaimMappings, claimMapping => claimMapping.Key == "HourlyRate"
      && claimMapping.Name == "rate" && claimMapping.Type == "float");

    Assert.Equal(3, realm.CustomAttributes.Count());
    Assert.Contains(realm.CustomAttributes, customAttribute => customAttribute.Key == "LocationKind"
      && customAttribute.Value == "ShoppingMall");
    Assert.Contains(realm.CustomAttributes, customAttribute => customAttribute.Key == "PhoneNumber"
      && customAttribute.Value == "+15148454636");
    Assert.Contains(realm.CustomAttributes, customAttribute => customAttribute.Key == "PostalAddress"
      && customAttribute.Value == "150 Saint-Catherine St W, Montreal, Quebec H2X 3Y2");

    Assert.NotNull(realm.PasswordRecoverySender);
    Assert.Equal(_sender.Id.ToGuid(), realm.PasswordRecoverySender.Id);

    Assert.NotNull(realm.PasswordRecoveryTemplate);
    Assert.Equal(_template.Id.ToGuid(), realm.PasswordRecoveryTemplate.Id);
  }

  [Fact(DisplayName = "ReplaceAsync: it should return null when the realm is not found.")]
  public async Task ReplaceAsync_it_should_return_null_when_the_realm_is_not_found()
  {
    ReplaceRealmPayload payload = new();
    Assert.Null(await _realmService.ReplaceAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "ReplaceAsync: it should throw AggregateNotFoundException when the sender could not be found.")]
  public async Task ReplaceAsync_it_should_throw_AggregateNotFoundException_when_the_sender_could_not_be_found()
  {
    ReplaceRealmPayload payload = new()
    {
      UniqueSlug = _realm.UniqueSlug,
      DisplayName = _realm.DisplayName,
      Description = _realm.Description,
      DefaultLocale = _realm.DefaultLocale?.Code,
      Secret = _realm.Secret,
      Url = _realm.Url?.ToString(),
      RequireUniqueEmail = _realm.RequireUniqueEmail,
      RequireConfirmedAccount = _realm.RequireConfirmedAccount,
      UniqueNameSettings = new UniqueNameSettings
      {
        AllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"
      },
      PasswordSettings = new PasswordSettings
      {
        RequiredLength = 7,
        RequiredUniqueChars = 4,
        RequireNonAlphanumeric = true,
        RequireLowercase = true,
        RequireUppercase = true,
        RequireDigit = false,
        Strategy = "PBKDF2"
      },
      PasswordRecoverySenderId = Guid.Empty,
      PasswordRecoveryTemplateId = _template.Id.ToGuid()
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<SenderAggregate>>(async () => await _realmService.ReplaceAsync(_realm.Id.ToGuid(), payload));
    Assert.Equal(payload.PasswordRecoverySenderId?.ToString(), exception.Id);
    Assert.Equal(nameof(payload.PasswordRecoverySenderId), exception.PropertyName);
  }

  [Fact(DisplayName = "ReplaceAsync: it should throw AggregateNotFoundException when the template could not be found.")]
  public async Task ReplaceAsync_it_should_throw_AggregateNotFoundException_when_the_template_could_not_be_found()
  {
    ReplaceRealmPayload payload = new()
    {
      UniqueSlug = _realm.UniqueSlug,
      DisplayName = _realm.DisplayName,
      Description = _realm.Description,
      DefaultLocale = _realm.DefaultLocale?.Code,
      Secret = _realm.Secret,
      Url = _realm.Url?.ToString(),
      RequireUniqueEmail = _realm.RequireUniqueEmail,
      RequireConfirmedAccount = _realm.RequireConfirmedAccount,
      UniqueNameSettings = new UniqueNameSettings
      {
        AllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"
      },
      PasswordSettings = new PasswordSettings
      {
        RequiredLength = 7,
        RequiredUniqueChars = 4,
        RequireNonAlphanumeric = true,
        RequireLowercase = true,
        RequireUppercase = true,
        RequireDigit = false,
        Strategy = "PBKDF2"
      },
      PasswordRecoverySenderId = _sender.Id.ToGuid(),
      PasswordRecoveryTemplateId = Guid.Empty
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<TemplateAggregate>>(async () => await _realmService.ReplaceAsync(_realm.Id.ToGuid(), payload));
    Assert.Equal(payload.PasswordRecoveryTemplateId?.ToString(), exception.Id);
    Assert.Equal(nameof(payload.PasswordRecoveryTemplateId), exception.PropertyName);
  }

  [Fact(DisplayName = "ReplaceAsync: it should throw UniqueSlugAlreadyUsedException when the unique slug is already used.")]
  public async Task ReplaceAsync_it_should_throw_UniqueSlugAlreadyUsedException_when_the_unique_slug_is_already_used()
  {
    RealmAggregate realm = new($"{_realm.UniqueSlug}-2");
    await AggregateRepository.SaveAsync(realm);

    ReplaceRealmPayload payload = new()
    {
      UniqueSlug = $" {realm.UniqueSlug} ",
      PasswordSettings = new PasswordSettings
      {
        RequiredLength = 1,
        RequiredUniqueChars = 1,
        Strategy = "BCRYPT"
      }
    };

    var exception = await Assert.ThrowsAsync<UniqueSlugAlreadyUsedException>(async () => await _realmService.ReplaceAsync(_realm.Id.ToGuid(), payload));
    Assert.Equal(payload.UniqueSlug, exception.UniqueSlug);
    Assert.Equal(nameof(payload.UniqueSlug), exception.PropertyName);
  }

  [Fact(DisplayName = "SearchAsync: it should return empty results when none are matching.")]
  public async Task SearchAsync_it_should_return_empty_results_when_none_are_matching()
  {
    SearchRealmsPayload payload = new()
    {
      IdIn = new[] { Guid.Empty }
    };

    SearchResults<Realm> results = await _realmService.SearchAsync(payload);

    Assert.Empty(results.Results);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    RealmAggregate idNotIn = new("desjardins-123");
    RealmAggregate realm1 = new("legacy-desjcoop");
    RealmAggregate realm2 = new("desjardins-514");
    RealmAggregate realm3 = new("desjzrdins-450");
    RealmAggregate realm4 = new("désjardîns-987");
    await AggregateRepository.SaveAsync(new[] { idNotIn, realm1, realm2, realm3, realm4 });

    RealmAggregate[] realms = new[] { realm1, realm2, realm3, realm4 }
      .OrderBy(x => x.DisplayName).Skip(1).Take(2).ToArray();

    HashSet<Guid> ids = (await PortalContext.Realms.AsNoTracking().ToArrayAsync())
      .Select(realm => new AggregateId(realm.AggregateId).ToGuid()).ToHashSet();
    ids.Remove(idNotIn.Id.ToGuid());

    SearchRealmsPayload payload = new()
    {
      Search = new TextSearch
      {
        Operator = SearchOperator.Or,
        Terms = new SearchTerm[]
        {
          new("d_sj_rd_ns-%"),
          new("%legacy%")
        }
      },
      IdIn = ids,
      Sort = new RealmSortOption[]
      {
        new RealmSortOption(RealmSort.DisplayName)
      },
      Skip = 1,
      Limit = 2
    };

    SearchResults<Realm> results = await _realmService.SearchAsync(payload);

    Assert.Equal(realms.Length, results.Results.Count());
    Assert.Equal(4, results.Total);

    for (int i = 0; i < realms.Length; i++)
    {
      Assert.Equal(realms[i].Id.ToGuid(), results.Results.ElementAt(i).Id);
    }
  }

  [Fact(DisplayName = "UpdateAsync: it should return null when the realm is not found.")]
  public async Task UpdateAsync_it_should_return_null_when_the_realm_is_not_found()
  {
    UpdateRealmPayload payload = new();
    Assert.Null(await _realmService.UpdateAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "UpdateAsync: it should throw AggregateNotFoundException when the sender could not be found.")]
  public async Task UpdateAsync_it_should_throw_AggregateNotFoundException_when_the_sender_could_not_be_found()
  {
    UpdateRealmPayload payload = new()
    {
      PasswordRecoverySenderId = new Modification<Guid?>(Guid.Empty)
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<SenderAggregate>>(async () => await _realmService.UpdateAsync(_realm.Id.ToGuid(), payload));
    Assert.Equal(payload.PasswordRecoverySenderId.Value?.ToString(), exception.Id);
    Assert.Equal(nameof(payload.PasswordRecoverySenderId), exception.PropertyName);
  }

  [Fact(DisplayName = "UpdateAsync: it should throw AggregateNotFoundException when the template could not be found.")]
  public async Task UpdateAsync_it_should_throw_AggregateNotFoundException_when_the_template_could_not_be_found()
  {
    UpdateRealmPayload payload = new()
    {
      PasswordRecoveryTemplateId = new Modification<Guid?>(Guid.Empty)
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<TemplateAggregate>>(async () => await _realmService.UpdateAsync(_realm.Id.ToGuid(), payload));
    Assert.Equal(payload.PasswordRecoveryTemplateId.Value?.ToString(), exception.Id);
    Assert.Equal(nameof(payload.PasswordRecoveryTemplateId), exception.PropertyName);
  }

  [Fact(DisplayName = "UpdateAsync: it should throw UniqueSlugAlreadyUsedException when the unique slug is already used.")]
  public async Task UpdateAsync_it_should_throw_UniqueSlugAlreadyUsedException_when_the_unique_slug_is_already_used()
  {
    RealmAggregate realm = new($"{_realm.UniqueSlug}-2");
    await AggregateRepository.SaveAsync(realm);

    UpdateRealmPayload payload = new()
    {
      UniqueSlug = $" {realm.UniqueSlug} "
    };

    var exception = await Assert.ThrowsAsync<UniqueSlugAlreadyUsedException>(async () => await _realmService.UpdateAsync(_realm.Id.ToGuid(), payload));
    Assert.Equal(payload.UniqueSlug, exception.UniqueSlug);
    Assert.Equal(nameof(payload.UniqueSlug), exception.PropertyName);
  }

  [Fact(DisplayName = "UpdateAsync: it should update the realm.")]
  public async Task UpdateAsync_it_should_update_the_realm()
  {
    _realm.SetPasswordRecoverySender(_sender);
    _realm.SetPasswordRecoveryTemplate(_template);
    await AggregateRepository.SaveAsync(_realm);

    UpdateRealmPayload payload = new()
    {
      Description = new Modification<string>("Indoor complex featuring shops with apparel, accessories & housewares, plus a food court & a grocer."),
      Secret = "jVf5!.UNxR6ny?$K+S~T2Wh-8@_4d;v'",
      RequireUniqueEmail = true,
      PasswordSettings = new PasswordSettings
      {
        RequiredLength = 7,
        RequiredUniqueChars = 4,
        RequireNonAlphanumeric = true,
        RequireLowercase = true,
        RequireUppercase = true,
        RequireDigit = false,
        Strategy = "BCRYPT"
      },
      ClaimMappings = new ClaimMappingModification[]
      {
        new("EmployeeId", name: null),
        new("EmployeeNo", "employee_no", "string")
      },
      CustomAttributes = new CustomAttributeModification[]
      {
        new("PostalCode", value: null),
        new("  PostalAddress  ", "  150 Saint-Catherine St W, Montreal, Quebec H2X 3Y2  ")
      },
      PasswordRecoverySenderId = new Modification<Guid?>(null),
      PasswordRecoveryTemplateId = new Modification<Guid?>(null)
    };

    Realm? realm = await _realmService.UpdateAsync(_realm.Id.ToGuid(), payload);
    Assert.NotNull(realm);

    Assert.Equal(_realm.Id.ToGuid(), realm.Id);
    Assert.Equal(Guid.Empty, realm.CreatedBy.Id);
    AssertEqual(_realm.CreatedOn, realm.CreatedOn);
    Assert.Equal(Actor, realm.UpdatedBy);
    AssertIsNear(realm.UpdatedOn);
    Assert.True(realm.Version > 1);

    Assert.Equal(payload.Description.Value, realm.Description);
    Assert.Equal(payload.Secret, realm.Secret);
    Assert.Equal(payload.RequireUniqueEmail, realm.RequireUniqueEmail);
    Assert.Equal(payload.PasswordSettings, realm.PasswordSettings);

    Assert.Equal(2, realm.ClaimMappings.Count());
    Assert.Contains(realm.ClaimMappings, claimMapping => claimMapping.Key == "EmployeeNo"
      && claimMapping.Name == "employee_no" && claimMapping.Type == "string");
    Assert.Contains(realm.ClaimMappings, claimMapping => claimMapping.Key == "HourlyRate"
      && claimMapping.Name == "rate" && claimMapping.Type == "double");

    Assert.Equal(2, realm.CustomAttributes.Count());
    Assert.Contains(realm.CustomAttributes, customAttribute => customAttribute.Key == "PhoneNumber"
      && customAttribute.Value == "+15148454636");
    Assert.Contains(realm.CustomAttributes, customAttribute => customAttribute.Key == "PostalAddress"
      && customAttribute.Value == "150 Saint-Catherine St W, Montreal, Quebec H2X 3Y2");

    Assert.Null(realm.PasswordRecoverySender);
    Assert.Null(realm.PasswordRecoveryTemplate);
  }
}
