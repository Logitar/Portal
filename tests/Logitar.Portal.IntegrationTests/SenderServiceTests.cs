using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class SenderServiceTests : IntegrationTests, IAsyncLifetime
{
  private readonly ISenderService _senderService;

  private readonly RealmAggregate _realm;
  private readonly SenderAggregate _sender;

  public SenderServiceTests() : base()
  {
    _senderService = ServiceProvider.GetRequiredService<ISenderService>();

    _realm = new("logitar");

    _sender = new(Faker.Person.Email, ProviderType.SendGrid, isDefault: true, tenantId: _realm.Id.Value)
    {
      DisplayName = Faker.Person.FullName
    };
    _sender.SetSetting("ApiKey", "SG.ABC.1234567890");
    _sender.SetSetting("BaseUrl", "https://api.sendgrid.com/v3/mail/send");
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(new AggregateRoot[] { _realm, _sender });
  }

  [Fact(DisplayName = "CreateAsync: it should create a sender.")]
  public async Task CreateAsync_it_should_create_a_sender()
  {
    CreateSenderPayload payload = new()
    {
      Realm = $"  {_realm.UniqueSlug.ToUpper()}  ",
      EmailAddress = $"  {Faker.Internet.Email()}  ",
      DisplayName = $"  {Faker.Name.FullName()}  ",
      Description = "    ",
      Provider = ProviderType.SendGrid,
      Settings = new ProviderSetting[]
      {
        new("  ApiKey  ", "  SG.ABC.1234567890  "),
        new("BaseUrl", "https://api.sendgrid.com/v3/mail/send")
      }
    };

    Sender? sender = await _senderService.CreateAsync(payload);
    Assert.NotNull(sender);

    Assert.NotEqual(Guid.Empty, sender.Id);
    Assert.Equal(Actor, sender.CreatedBy);
    AssertIsNear(sender.CreatedOn);
    Assert.Equal(Actor, sender.UpdatedBy);
    AssertIsNear(sender.UpdatedOn);
    Assert.True(sender.Version >= 1);

    Assert.False(sender.IsDefault);
    Assert.Equal(payload.EmailAddress.Trim(), sender.EmailAddress);
    Assert.Equal(payload.DisplayName.CleanTrim(), sender.DisplayName);
    Assert.Equal(payload.Description.CleanTrim(), sender.Description);
    Assert.Equal(payload.Provider, sender.Provider);

    Assert.Equal(2, sender.Settings.Count());
    Assert.Contains(sender.Settings, setting => setting.Key == "ApiKey"
      && setting.Value == "SG.ABC.1234567890");
    Assert.Contains(sender.Settings, setting => setting.Key == "BaseUrl"
      && setting.Value == "https://api.sendgrid.com/v3/mail/send");

    Assert.NotNull(sender.Realm);
    Assert.Equal(_realm.Id.ToGuid(), sender.Realm.Id);
  }

  [Fact(DisplayName = "CreateAsync: it should throw AggregateNotFoundException when the realm is not found.")]
  public async Task CreateAsync_it_should_throw_AggregateNotFoundException_when_the_realm_is_not_found()
  {
    CreateSenderPayload payload = new()
    {
      Realm = Guid.Empty.ToString()
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<RealmAggregate>>(async () => await _senderService.CreateAsync(payload));
    Assert.Equal(payload.Realm, exception.Id);
    Assert.Equal(nameof(payload.Realm), exception.PropertyName);
  }

  [Fact(DisplayName = "DeleteAsync: it should delete the sender.")]
  public async Task DeleteAsync_it_should_delete_the_sender()
  {
    SenderAggregate sender = new(_sender.EmailAddress, _sender.Provider, isDefault: false, _sender.TenantId);
    await AggregateRepository.SaveAsync(sender);

    Sender? deleted = await _senderService.DeleteAsync(sender.Id.ToGuid());
    Assert.NotNull(deleted);
    Assert.Equal(sender.Id.ToGuid(), deleted.Id);

    deleted = await _senderService.DeleteAsync(_sender.Id.ToGuid());
    Assert.NotNull(deleted);
    Assert.Equal(_sender.Id.ToGuid(), deleted.Id);
  }

  [Fact(DisplayName = "DeleteAsync: it should return null when the sender is not found.")]
  public async Task DeleteAsync_it_should_return_null_when_the_sender_is_not_found()
  {
    Assert.Null(await _senderService.DeleteAsync(Guid.Empty));
  }

  [Fact(DisplayName = "DeleteAsync: it should throw CannotDeleteDefaultSenderException when deleting the default sender when it is not alone.")]
  public async Task DeleteAsync_it_should_throw_CannotDeleteDefaultSenderException_when_deleting_the_default_sender_when_it_is_not_alone()
  {
    SenderAggregate other = new(_sender.EmailAddress, _sender.Provider, isDefault: false, _sender.TenantId);
    await AggregateRepository.SaveAsync(new AggregateRoot[] { other });

    var exception = await Assert.ThrowsAsync<CannotDeleteDefaultSenderException>(async () => await _senderService.DeleteAsync(_sender.Id.ToGuid()));
    Assert.Equal(_sender.ToString(), exception.Sender);
  }

  [Fact(DisplayName = "ReadAsync: it should return null when the sender is not found.")]
  public async Task ReadAsync_it_should_return_null_when_the_sender_is_not_found()
  {
    Assert.Null(await _senderService.ReadAsync(Guid.Empty));
  }

  [Fact(DisplayName = "ReadAsync: it should return the sender found by ID.")]
  public async Task ReadAsync_it_should_return_the_sender_found_by_Id()
  {
    Sender? sender = await _senderService.ReadAsync(_sender.Id.ToGuid());
    Assert.NotNull(sender);
    Assert.Equal(_sender.Id.ToGuid(), sender.Id);
  }

  [Fact(DisplayName = "ReadDefaultAsync: it should return null when the default sender is not found.")]
  public async Task ReadDefaultAsync_it_should_return_null_when_the_default_sender_is_not_found()
  {
    Assert.Null(await _senderService.ReadDefaultAsync(realm: null));
  }

  [Fact(DisplayName = "ReadDefaultAsync: it should return the found default sender.")]
  public async Task ReadDefaultAsync_it_should_return_the_found_default_sender()
  {
    Sender? sender = await _senderService.ReadDefaultAsync($" {_realm.Id.ToGuid()} ");
    Assert.NotNull(sender);
    Assert.Equal(_sender.Id.ToGuid(), sender.Id);
  }

  [Fact(DisplayName = "ReplaceAsync: it should replace the sender.")]
  public async Task ReplaceAsync_it_should_replace_the_sender()
  {
    long version = _sender.Version;

    _sender.SetSetting("Basic", "dGVzdDpIZWxsbyBXb3JsZCE=");
    await AggregateRepository.SaveAsync(_sender);

    ReplaceSenderPayload payload = new()
    {
      EmailAddress = $"  {Faker.Internet.Email()}  ",
      DisplayName = $"  {Faker.Name.FullName()}  ",
      Description = "  This is the sender used per default in this realm.  ",
      Settings = new ProviderSetting[]
      {
        new("BaseUrl", "https://api.sendgrid.com/")
      }
    };

    Sender? sender = await _senderService.ReplaceAsync(_sender.Id.ToGuid(), payload, version);

    Assert.NotNull(sender);

    Assert.Equal(_sender.Id.ToGuid(), sender.Id);
    Assert.Equal(Guid.Empty, sender.CreatedBy.Id);
    AssertEqual(_sender.CreatedOn, sender.CreatedOn);
    Assert.Equal(Actor, sender.UpdatedBy);
    AssertIsNear(sender.UpdatedOn);
    Assert.True(sender.Version > 1);

    Assert.Equal(payload.EmailAddress.Trim(), sender.EmailAddress);
    Assert.Equal(payload.DisplayName.Trim(), sender.DisplayName);
    Assert.Equal(payload.Description.Trim(), sender.Description);

    Assert.Equal(2, sender.Settings.Count());
    Assert.Contains(sender.Settings, setting => setting.Key == "BaseUrl"
      && setting.Value == "https://api.sendgrid.com/");
    Assert.Contains(sender.Settings, setting => setting.Key == "Basic"
      && setting.Value == "dGVzdDpIZWxsbyBXb3JsZCE=");
  }

  [Fact(DisplayName = "ReplaceAsync: it should return null when the sender is not found.")]
  public async Task ReplaceAsync_it_should_return_null_when_the_sender_is_not_found()
  {
    ReplaceSenderPayload payload = new();
    Assert.Null(await _senderService.ReplaceAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "SearchAsync: it should return empty results when none are matching.")]
  public async Task SearchAsync_it_should_return_empty_results_when_none_are_matching()
  {
    SearchSendersPayload payload = new()
    {
      IdIn = new[] { Guid.Empty }
    };

    SearchResults<Sender> results = await _senderService.SearchAsync(payload);

    Assert.Empty(results.Results);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    string tenantId = _realm.Id.Value;

    SenderAggregate notInRealm = new(Faker.Internet.Email(), ProviderType.SendGrid, isDefault: false);
    SenderAggregate idNotIn = new(Faker.Internet.Email(), ProviderType.SendGrid, isDefault: false, tenantId);
    SenderAggregate notSendGrid = new(Faker.Internet.Email(), (ProviderType)(-1), isDefault: false, tenantId);
    SenderAggregate sender1 = new(Faker.Internet.Email(), ProviderType.SendGrid, isDefault: false, tenantId)
    {
      DisplayName = "Logitar"
    };
    SenderAggregate sender2 = new(Faker.Internet.Email(), ProviderType.SendGrid, isDefault: false, tenantId)
    {
      DisplayName = "Logistic"
    };
    SenderAggregate sender3 = new(Faker.Internet.Email(), ProviderType.SendGrid, isDefault: false, tenantId)
    {
      DisplayName = "Logo"
    };
    SenderAggregate sender4 = new(Faker.Internet.Email(), ProviderType.SendGrid, isDefault: false, tenantId)
    {
      DisplayName = "Logging"
    };
    await AggregateRepository.SaveAsync(new[] { notInRealm, idNotIn, notSendGrid, sender1, sender2, sender3, sender4 });

    SenderAggregate[] senders = new[] { sender1, sender2, sender3, sender4 }
      .OrderBy(x => x.DisplayName).Skip(1).Take(2).ToArray();

    HashSet<Guid> ids = (await PortalContext.Senders.AsNoTracking().ToArrayAsync())
      .Select(sender => new AggregateId(sender.AggregateId).ToGuid()).ToHashSet();
    ids.Remove(idNotIn.Id.ToGuid());

    SearchSendersPayload payload = new()
    {
      Search = new TextSearch
      {
        Operator = SearchOperator.Or,
        Terms = new SearchTerm[]
        {
          new("l_g_%"),
          new(Guid.NewGuid().ToString())
        }
      },
      IdIn = ids,
      Provider = ProviderType.SendGrid,
      Realm = $" {_realm.UniqueSlug.ToUpper()} ",
      Sort = new SenderSortOption[]
      {
        new(SenderSort.DisplayName)
      },
      Skip = 1,
      Limit = 2
    };

    SearchResults<Sender> results = await _senderService.SearchAsync(payload);

    Assert.Equal(senders.Length, results.Results.Count());
    Assert.Equal(4, results.Total);

    for (int i = 0; i < senders.Length; i++)
    {
      Assert.Equal(senders[i].Id.ToGuid(), results.Results.ElementAt(i).Id);
    }
  }

  [Fact(DisplayName = "SetDefaultAsync: it should return null when the sender is not found.")]
  public async Task SetDefaultAsync_it_should_return_null_when_the_sender_is_not_found()
  {
    Assert.Null(await _senderService.SetDefaultAsync(Guid.Empty));
  }

  [Fact(DisplayName = "SetDefaultAsync: it should set the default sender.")]
  public async Task SetDefaultAsync_it_should_set_the_default_sender()
  {
    SenderAggregate sender = new(Faker.Internet.Email(), ProviderType.SendGrid, isDefault: false, tenantId: _realm.Id.Value);
    await AggregateRepository.SaveAsync(sender);

    Sender? result = await _senderService.SetDefaultAsync(sender.Id.ToGuid());
    Assert.NotNull(result);
    Assert.True(result.IsDefault);

    Assert.Equal(sender.Id.ToGuid(), result.Id);
    Assert.Equal(Guid.Empty, result.CreatedBy.Id);
    AssertEqual(sender.CreatedOn, result.CreatedOn);
    Assert.Equal(Actor, result.UpdatedBy);
    AssertIsNear(result.UpdatedOn);
    Assert.True(result.Version > sender.Version);

    SenderAggregate? aggregate = await AggregateRepository.LoadAsync<SenderAggregate>(_sender.Id);
    Assert.NotNull(aggregate);
    Assert.False(aggregate.IsDefault);

    Assert.Equal(_sender.CreatedOn, aggregate.CreatedOn);
    Assert.Equal(_sender.CreatedBy, aggregate.CreatedBy);
    Assert.Equal(ActorId, aggregate.UpdatedBy);
    AssertIsNear(aggregate.UpdatedOn);
    Assert.True(aggregate.Version > _sender.Version);
  }

  [Fact(DisplayName = "UpdateAsync: it should return null when the sender is not found.")]
  public async Task UpdateAsync_it_should_return_null_when_the_sender_is_not_found()
  {
    UpdateSenderPayload payload = new();
    Assert.Null(await _senderService.UpdateAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "UpdateAsync: it should update the sender.")]
  public async Task UpdateAsync_it_should_update_the_sender()
  {
    UpdateSenderPayload payload = new()
    {
      Description = new Modification<string>("  This is the sender used per default in this realm.  "),
      Settings = new ProviderSettingModification[]
      {
        new("ApiKey", value: null),
        new("BaseUrl", "https://api.sendgrid.com/"),
        new("Basic", "dGVzdDpIZWxsbyBXb3JsZCE=")
      }
    };

    Sender? sender = await _senderService.UpdateAsync(_sender.Id.ToGuid(), payload);

    Assert.NotNull(sender);

    Assert.Equal(_sender.Id.ToGuid(), sender.Id);
    Assert.Equal(Guid.Empty, sender.CreatedBy.Id);
    AssertEqual(_sender.CreatedOn, sender.CreatedOn);
    Assert.Equal(Actor, sender.UpdatedBy);
    AssertIsNear(sender.UpdatedOn);
    Assert.True(sender.Version > 1);

    Assert.Equal(payload.Description.Value?.CleanTrim(), sender.Description);

    Assert.Equal(2, sender.Settings.Count());
    Assert.Contains(sender.Settings, setting => setting.Key == "BaseUrl"
      && setting.Value == "https://api.sendgrid.com/");
    Assert.Contains(sender.Settings, setting => setting.Key == "Basic"
      && setting.Value == "dGVzdDpIZWxsbyBXb3JsZCE=");
  }
}
