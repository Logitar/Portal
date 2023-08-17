using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Logitar.Portal.Services;

[Trait(Traits.Category, Categories.Integration)]
public class RealmServiceTests : IntegrationTestBase, IAsyncLifetime
{
  private readonly IRealmRepository _realmRepository;
  private readonly IRealmService _realmService;

  private readonly RealmAggregate _realm;

  public RealmServiceTests()
  {
    _realmRepository = ServiceProvider.GetRequiredService<IRealmRepository>();
    _realmService = ServiceProvider.GetRequiredService<IRealmService>();

    _realm = new("skillcraft", new ActorId(Actor.Id))
    {
      DisplayName = "SkillCraft",
      Description = "The realm for the SkillCraft RPG web application.",
      DefaultLocale = CultureInfo.GetCultureInfo("fr-CA"),
      Url = new Uri("https://skillcraft.francispion.ca/"),
      RequireUniqueEmail = true,
      RequireConfirmedAccount = true
    };
    _realm.SetClaimMapping("PlayerId", new ReadOnlyClaimMapping("player_id", "Guid"));
    _realm.SetClaimMapping("IsArchived", new ReadOnlyClaimMapping("archived", "Boolean"));
    _realm.SetCustomAttribute("WebhookUrl", "https://skillcraft.francispion.ca/webhooks/portal");
    _realm.SetCustomAttribute("WebhookUsername", "portal");
  }

  [Fact(DisplayName = "CreateAsync: it should create the realm.")]
  public async Task CreateAsync_it_should_create_the_realm()
  {
    CreateRealmPayload payload = new()
    {
      UniqueSlug = $"{_realm.UniqueSlug}2",
      DisplayName = $" {_realm.DisplayName} ",
      Description = $" {_realm.Description} ",
      DefaultLocale = $" {_realm.DefaultLocale?.Name} ",
      Secret = _realm.Secret,
      Url = $" {_realm.Url?.ToString()} ",
      RequireUniqueEmail = _realm.RequireUniqueEmail,
      RequireConfirmedAccount = _realm.RequireConfirmedAccount,
      UniqueNameSettings = new()
      {
        AllowedCharacters = _realm.UniqueNameSettings.AllowedCharacters
      },
      PasswordSettings = new()
      {
        RequiredLength = _realm.PasswordSettings.RequiredLength,
        RequiredUniqueChars = _realm.PasswordSettings.RequiredUniqueChars,
        RequireNonAlphanumeric = _realm.PasswordSettings.RequireNonAlphanumeric,
        RequireLowercase = _realm.PasswordSettings.RequireLowercase,
        RequireUppercase = _realm.PasswordSettings.RequireUppercase,
        RequireDigit = _realm.PasswordSettings.RequireDigit
      },
      ClaimMappings = _realm.ClaimMappings.Select(
        claimMapping => new ClaimMapping(claimMapping.Key, claimMapping.Value.Name, claimMapping.Value.Type)
      ),
      CustomAttributes = _realm.CustomAttributes.Select(
        customAttribute => new CustomAttribute(customAttribute.Key, customAttribute.Value)
      )
    };

    Realm realm = await _realmService.CreateAsync(payload);
    Assert.NotNull(realm);
    Assert.Equal(new Actor()/*Actor*/, realm.CreatedBy); // TODO(fpion): resolve actor
    Assert.Equal(new Actor()/*Actor*/, realm.UpdatedBy); // TODO(fpion): resolve actor
    Assert.Equal(payload.UniqueSlug, realm.UniqueSlug);
    Assert.Equal(payload.DisplayName.Trim(), realm.DisplayName);
    Assert.Equal(payload.Description.Trim(), realm.Description);
    Assert.Equal(payload.DefaultLocale.Trim(), realm.DefaultLocale);
    Assert.Equal(payload.Secret, realm.Secret);
    Assert.Equal(payload.Url.Trim(), realm.Url);
    Assert.Equal(payload.RequireUniqueEmail, realm.RequireUniqueEmail);
    Assert.Equal(payload.RequireConfirmedAccount, realm.RequireConfirmedAccount);
    Assert.Equal(payload.UniqueNameSettings, realm.UniqueNameSettings);
    Assert.Equal(payload.PasswordSettings, realm.PasswordSettings);
    Assert.Equal(payload.ClaimMappings, realm.ClaimMappings);
    Assert.Equal(payload.CustomAttributes, realm.CustomAttributes);
  }

  [Fact(DisplayName = "DeleteAsync: it should delete the realm.")]
  public async Task DeleteAsync_it_should_delete_the_realm()
  {
    Realm? realm = await _realmService.DeleteAsync(_realm.Id.Value);
    Assert.NotNull(realm);
    Assert.Equal(_realm.Id.Value, realm.Id);

    Assert.Null(await _realmRepository.LoadAsync(_realm.Id));
    Assert.Empty(PortalContext.Realms.Where(x => x.AggregateId == _realm.Id.Value));
  }

  [Fact(DisplayName = "ReadAsync: it should read the realm.")]
  public async Task ReadAsync_it_should_read_the_realm()
  {
    Realm? realm = await _realmService.ReadAsync(_realm.Id.Value);
    Assert.NotNull(realm);
    Assert.Equal(_realm.Id.Value, realm.Id);
  }

  [Fact(DisplayName = "ReplaceAsync: it should replace the realm.")]
  public async Task ReplaceAsync_it_should_replace_the_realm()
  {
    long version = _realm.Version;

    _realm.RemoveClaimMapping("PlayerId");
    _realm.SetClaimMapping("SubscriptionId", new ReadOnlyClaimMapping("subscription_id"));
    _realm.RemoveCustomAttribute("WebhookUsername");
    _realm.SetCustomAttribute("WebhookEndpoint", "https://skillcraft.francispion.ca/webhooks/portal");
    await _realmRepository.SaveAsync(_realm);

    ReplaceRealmPayload payload = new()
    {
      UniqueSlug = _realm.UniqueSlug,
      DisplayName = $" {_realm.DisplayName} ",
      Description = "  ",
      DefaultLocale = "en-CA",
      Secret = " ",
      Url = " https://skillcraft.francispion.ca/ ",
      RequireUniqueEmail = false,
      RequireConfirmedAccount = true,
      UniqueNameSettings = new()
      {
        AllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._~"
      },
      PasswordSettings = new(),
      ClaimMappings = new ClaimMapping[]
      {
        new("SubscriptionPlan", "subscription_plan"),
        new("PlayerId", "player_id", type: null)
      },
      CustomAttributes = new CustomAttribute[]
      {
        new("WebhookUsername", "skillcraft"),
        new("WebhookPassword", "P@s$W0rd")
      }
    };

    Realm? realm = await _realmService.ReplaceAsync(_realm.Id.Value, payload, version);
    Assert.NotNull(realm);
    Assert.Equal(new Actor()/*Actor*/, realm.UpdatedBy); // TODO(fpion): resolve actor
    Assert.Equal(payload.UniqueSlug, realm.UniqueSlug);
    Assert.Equal(payload.DisplayName.Trim(), realm.DisplayName);
    Assert.Null(realm.Description);
    Assert.Equal("en-CA", realm.DefaultLocale);
    Assert.NotEqual(_realm.Secret, realm.Secret);
    Assert.Equal(_realm.Url?.ToString(), realm.Url);
    Assert.Equal(payload.RequireUniqueEmail, realm.RequireUniqueEmail);
    Assert.Equal(payload.RequireConfirmedAccount, realm.RequireConfirmedAccount);
    Assert.Equal(payload.UniqueNameSettings, realm.UniqueNameSettings);
    Assert.Equal(payload.PasswordSettings, realm.PasswordSettings);

    Assert.Equal(3, realm.ClaimMappings.Count());
    Assert.Contains(realm.ClaimMappings, x => x.Key == "SubscriptionPlan"
      && x.Name == "subscription_plan" && x.Type == null);
    Assert.Contains(realm.ClaimMappings, x => x.Key == "PlayerId"
      && x.Name == "player_id" && x.Type == null);
    Assert.Contains(realm.ClaimMappings, x => x.Key == "SubscriptionId"
      && x.Name == "subscription_id" && x.Type == null);

    Assert.Equal(3, realm.CustomAttributes.Count());
    Assert.Contains(realm.CustomAttributes, x => x.Key == "WebhookUsername" && x.Value == "skillcraft");
    Assert.Contains(realm.CustomAttributes, x => x.Key == "WebhookPassword" && x.Value == "P@s$W0rd");
    Assert.Contains(realm.CustomAttributes, x => x.Key == "WebhookEndpoint" && x.Value == "https://skillcraft.francispion.ca/webhooks/portal");
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct realms.")]
  public async Task SearchAsync_it_should_return_the_correct_realms()
  {
    RealmAggregate devil2 = new("devil-ii")
    {
      DisplayName = "Devil II: Lords of Gehenna"
    };
    RealmAggregate mineCraft = new("minecraft")
    {
      DisplayName = "MineCraft"
    };
    RealmAggregate starCraft = new("starcraft")
    {
      DisplayName = "StarCraft"
    };
    RealmAggregate warCraft = new("warcraft")
    {
      DisplayName = "WarCraft"
    };
    await _realmRepository.SaveAsync(new[] { devil2, mineCraft, starCraft, warCraft });

    SearchRealmsPayload payload = new()
    {
      Search = new TextSearch
      {
        Terms = new SearchTerm[]
        {
          new("%legend%"),
          new("%craft%")
        },
        Operator = QueryOperator.Or
      },
      Sort = new RealmSortOption[]
      {
        new RealmSortOption(RealmSort.DisplayName)
      },
      Skip = 1,
      Limit = 2
    };

    SearchResults<Realm> results = await _realmService.SearchAsync(payload);
    Assert.Equal(4, results.Total);
    Assert.Equal(2, results.Results.Count());
    Assert.Equal(_realm.Id.Value, results.Results.ElementAt(0).Id);
    Assert.Equal(starCraft.Id.Value, results.Results.ElementAt(1).Id);
  }

  [Fact(DisplayName = "UpdateAsync: it should update the realm.")]
  public async Task UpdateAsync_it_should_update_the_realm()
  {
    UpdateRealmPayload payload = new()
    {
      UniqueSlug = $"{_realm.UniqueSlug}-2",
      Description = new MayBe<string>("    "),
      DefaultLocale = new MayBe<string>("en-CA"),
      RequireUniqueEmail = false,
      UniqueNameSettings = new()
      {
        AllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._~"
      },
      ClaimMappings = new ClaimMappingModification[]
      {
        new("SubscriptionPlan", "subscription_plan"),
        new("PlayerId", "player_id", type: null),
        new("IsArchived", name: null)
      },
      CustomAttributes = new CustomAttributeModification[]
      {
        new("WebhookUsername", "skillcraft"),
        new("WebhookPassword", "P@s$W0rd"),
        new("WebhookUrl", value: null)
      }
    };

    Realm? realm = await _realmService.UpdateAsync(_realm.Id.Value, payload);
    Assert.NotNull(realm);
    Assert.Equal(new Actor()/*Actor*/, realm.UpdatedBy); // TODO(fpion): resolve actor
    Assert.Equal(payload.UniqueSlug, realm.UniqueSlug);
    Assert.Equal(_realm.DisplayName, realm.DisplayName);
    Assert.Null(realm.Description);
    Assert.Equal("en-CA", realm.DefaultLocale);
    Assert.Equal(_realm.Secret, realm.Secret);
    Assert.Equal(_realm.Url?.ToString(), realm.Url);
    Assert.Equal(payload.RequireUniqueEmail, realm.RequireUniqueEmail);
    Assert.Equal(_realm.RequireConfirmedAccount, realm.RequireConfirmedAccount);
    Assert.Equal(payload.UniqueNameSettings, realm.UniqueNameSettings);
    Assert.Equal(_realm.PasswordSettings, realm.PasswordSettings.ToReadOnlyPasswordSettings());

    Assert.Equal(2, realm.ClaimMappings.Count());
    Assert.Contains(realm.ClaimMappings, x => x.Key == "SubscriptionPlan"
      && x.Name == "subscription_plan" && x.Type == null);
    Assert.Contains(realm.ClaimMappings, x => x.Key == "PlayerId"
      && x.Name == "player_id" && x.Type == null);

    Assert.Equal(2, realm.CustomAttributes.Count());
    Assert.Contains(realm.CustomAttributes, x => x.Key == "WebhookUsername" && x.Value == "skillcraft");
    Assert.Contains(realm.CustomAttributes, x => x.Key == "WebhookPassword" && x.Value == "P@s$W0rd");
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _realmRepository.SaveAsync(_realm);
  }
}
