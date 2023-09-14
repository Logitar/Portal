using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class TemplateServiceTests : IntegrationTests, IAsyncLifetime
{
  private readonly IRealmQuerier _realmQuerier;
  private readonly ITemplateService _templateService;

  private readonly RealmAggregate _realm;
  private readonly TemplateAggregate _template;

  public TemplateServiceTests() : base()
  {
    _realmQuerier = ServiceProvider.GetRequiredService<IRealmQuerier>();
    _templateService = ServiceProvider.GetRequiredService<ITemplateService>();

    _realm = new("logitar");

    _template = new(_realm.UniqueNameSettings, "PasswordRecovery", "PasswordRecovery_Subject",
      MediaTypeNames.Text.Plain, "Hello World!", tenantId: _realm.Id.Value);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(new AggregateRoot[] { _realm, _template });
  }

  [Fact(DisplayName = "CreateAsync: it should create a template.")]
  public async Task CreateAsync_it_should_create_a_template()
  {
    CreateTemplatePayload payload = new()
    {
      Realm = $"  {_realm.UniqueSlug.ToUpper()}  ",
      UniqueName = $"  {_template.UniqueName}2  ",
      Subject = $"  {_template.Subject}  ",
      ContentType = $"  {_template.ContentType}  ",
      Contents = $"  {_template.Contents}  "
    };

    Template? template = await _templateService.CreateAsync(payload);
    Assert.NotNull(template);

    Assert.NotEqual(Guid.Empty, template.Id);
    Assert.Equal(Actor, template.CreatedBy);
    AssertIsNear(template.CreatedOn);
    Assert.Equal(Actor, template.UpdatedBy);
    AssertIsNear(template.UpdatedOn);
    Assert.True(template.Version >= 1);

    Assert.Equal(payload.UniqueName.Trim(), template.UniqueName);
    Assert.Equal(payload.Subject.Trim(), template.Subject);
    Assert.Equal(payload.ContentType.Trim(), template.ContentType);
    Assert.Equal(payload.Contents.Trim(), template.Contents);

    Assert.NotNull(template.Realm);
    Assert.Equal(_realm.Id.ToGuid(), template.Realm.Id);
  }

  [Fact(DisplayName = "CreateAsync: it should throw AggregateNotFoundException when the realm is not found.")]
  public async Task CreateAsync_it_should_throw_AggregateNotFoundException_when_the_realm_is_not_found()
  {
    CreateTemplatePayload payload = new()
    {
      Realm = Guid.Empty.ToString()
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<RealmAggregate>>(async () => await _templateService.CreateAsync(payload));
    Assert.Equal(payload.Realm, exception.Id);
    Assert.Equal(nameof(payload.Realm), exception.PropertyName);
  }

  [Fact(DisplayName = "CreateAsync: it should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task CreateAsync_it_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    CreateTemplatePayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      UniqueName = _template.UniqueName,
      Subject = _template.Subject,
      ContentType = _template.ContentType,
      Contents = _template.Contents
    };

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<TemplateAggregate>>(async () => await _templateService.CreateAsync(payload));
    Assert.Equal(_realm.Id.Value, exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "DeleteAsync: it should delete the template.")]
  public async Task DeleteAsync_it_should_delete_the_template()
  {
    TemplateAggregate template = new(_realm.UniqueNameSettings, $"{_template.UniqueName}2",
      _template.Subject, _template.ContentType, _template.Contents, _template.TenantId);
    await AggregateRepository.SaveAsync(template);

    Template? deleted = await _templateService.DeleteAsync(template.Id.ToGuid());
    Assert.NotNull(deleted);
    Assert.Equal(template.Id.ToGuid(), deleted.Id);

    deleted = await _templateService.DeleteAsync(_template.Id.ToGuid());
    Assert.NotNull(deleted);
    Assert.Equal(_template.Id.ToGuid(), deleted.Id);
  }

  [Fact(DisplayName = "DeleteAsync: it should remove the realm password recovery template.")]
  public async Task DeleteAsync_it_should_remove_the_realm_password_recovery_template()
  {
    _realm.SetPasswordRecoveryTemplate(_template);
    await AggregateRepository.SaveAsync(_realm);

    Template? template = await _templateService.DeleteAsync(_template.Id.ToGuid());
    Assert.NotNull(template);
    Assert.Equal(_template.Id.ToGuid(), template.Id);

    RealmAggregate? aggregate = await AggregateRepository.LoadAsync<RealmAggregate>(_realm.Id);
    Assert.NotNull(aggregate);
    Assert.False(aggregate.PasswordRecoveryTemplateId.HasValue);

    Realm realm = await _realmQuerier.ReadAsync(_realm);
    Assert.Null(realm.PasswordRecoveryTemplate);
  }

  [Fact(DisplayName = "DeleteAsync: it should return null when the template is not found.")]
  public async Task DeleteAsync_it_should_return_null_when_the_template_is_not_found()
  {
    Assert.Null(await _templateService.DeleteAsync(Guid.Empty));
  }

  [Fact(DisplayName = "ReadAsync: it should return null when the template is not found.")]
  public async Task ReadAsync_it_should_return_null_when_the_template_is_not_found()
  {
    Assert.Null(await _templateService.ReadAsync(Guid.Empty));
  }

  [Fact(DisplayName = "ReadAsync: it should return the template found by ID.")]
  public async Task ReadAsync_it_should_return_the_template_found_by_Id()
  {
    Template? template = await _templateService.ReadAsync(_template.Id.ToGuid());
    Assert.NotNull(template);
    Assert.Equal(_template.Id.ToGuid(), template.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should return the template found by unique name.")]
  public async Task ReadAsync_it_should_return_the_template_found_by_unique_name()
  {
    Template? template = await _templateService.ReadAsync(realm: $" {_realm.Id.ToGuid()} ", uniqueName: $" {_template.UniqueName} ");
    Assert.NotNull(template);
    Assert.Equal(_template.Id.ToGuid(), template.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should throw TooManyResultsException when many templates have been found.")]
  public async Task ReadAsync_it_should_throw_TooManyResultsException_when_many_templates_have_been_found()
  {
    TemplateAggregate template = new(_realm.UniqueNameSettings, "CreateUser", "CreateUser_Subject", MediaTypeNames.Text.Plain, "CreateUser_Body");
    await AggregateRepository.SaveAsync(template);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<Template>>(
      async () => await _templateService.ReadAsync(template.Id.ToGuid(), _realm.UniqueSlug, _template.UniqueName)
    );
    Assert.Equal(1, exception.Expected);
    Assert.Equal(2, exception.Actual);
  }

  [Fact(DisplayName = "ReplaceAsync: it should replace the template.")]
  public async Task ReplaceAsync_it_should_replace_the_template()
  {
    long version = _template.Version;

    ReplaceTemplatePayload payload = new()
    {
      UniqueName = _template.UniqueName,
      DisplayName = "Password Recovery",
      Description = "This is the template used for password recovery.",
      Subject = _template.Subject,
      ContentType = _template.ContentType,
      Contents = "PasswordRecovery_Body"
    };

    Template? template = await _templateService.ReplaceAsync(_template.Id.ToGuid(), payload, version);

    Assert.NotNull(template);

    Assert.Equal(_template.Id.ToGuid(), template.Id);
    Assert.Equal(Guid.Empty, template.CreatedBy.Id);
    AssertEqual(_template.CreatedOn, template.CreatedOn);
    Assert.Equal(Actor, template.UpdatedBy);
    AssertIsNear(template.UpdatedOn);
    Assert.True(template.Version > 1);

    Assert.Equal(payload.UniqueName, template.UniqueName);
    Assert.Equal(payload.DisplayName, template.DisplayName);
    Assert.Equal(payload.Description, template.Description);
    Assert.Equal(payload.Subject, template.Subject);
    Assert.Equal(payload.ContentType, template.ContentType);
    Assert.Equal(payload.Contents, template.Contents);
  }

  [Fact(DisplayName = "ReplaceAsync: it should return null when the template is not found.")]
  public async Task ReplaceAsync_it_should_return_null_when_the_template_is_not_found()
  {
    ReplaceTemplatePayload payload = new();
    Assert.Null(await _templateService.ReplaceAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "ReplaceAsync: it should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task ReplaceAsync_it_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    TemplateAggregate template = new(_realm.UniqueNameSettings, $"{_template.UniqueName}2",
      _template.Subject, _template.ContentType, _template.Contents, _template.TenantId);
    await AggregateRepository.SaveAsync(template);

    ReplaceTemplatePayload payload = new()
    {
      UniqueName = $" {template.UniqueName} ",
      Subject = template.Subject,
      ContentType = template.ContentType,
      Contents = template.Contents
    };

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<TemplateAggregate>>(
      async () => await _templateService.ReplaceAsync(_template.Id.ToGuid(), payload)
    );
    Assert.Equal(template.TenantId, exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "SearchAsync: it should return empty results when none are matching.")]
  public async Task SearchAsync_it_should_return_empty_results_when_none_are_matching()
  {
    SearchTemplatesPayload payload = new()
    {
      IdIn = new[] { Guid.Empty }
    };

    SearchResults<Template> results = await _templateService.SearchAsync(payload);

    Assert.Empty(results.Results);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    string tenantId = _realm.Id.Value;

    TemplateAggregate notMatching = new(_realm.UniqueNameSettings, "test", "Test", MediaTypeNames.Text.Plain, "Hello World!", tenantId);
    TemplateAggregate notInRealm = new(_realm.UniqueNameSettings, _template.UniqueName, _template.Subject, _template.ContentType, _template.Contents);
    TemplateAggregate idNotIn = new(_realm.UniqueNameSettings, "AccountConfirmed", "AccountConfirmed_Subject", MediaTypeNames.Text.Plain, "AccountConfirmed_Body", tenantId);
    TemplateAggregate notPlainText = new(_realm.UniqueNameSettings, "AccountDeleted", "AccountDeleted_Subject", MediaTypeNames.Text.Html, "AccountDeleted_Body", tenantId);
    TemplateAggregate template1 = new(_realm.UniqueNameSettings, "CreateUser", "CreateUser_Subject", MediaTypeNames.Text.Plain, "CreateUser_Body", tenantId)
    {
      DisplayName = "Create User"
    };
    TemplateAggregate template2 = new(_realm.UniqueNameSettings, "VerifyEmail", "VerifyEmail_Subject", MediaTypeNames.Text.Plain, "VerifyEmail_Body", tenantId)
    {
      DisplayName = "Verify Email"
    };
    TemplateAggregate template3 = new(_realm.UniqueNameSettings, "VerifyPhone", "VerifyPhone_Subject", MediaTypeNames.Text.Plain, "VerifyPhone_Body", tenantId)
    {
      DisplayName = "Verify Phone"
    };
    await AggregateRepository.SaveAsync(new[] { notMatching, notInRealm, idNotIn, notPlainText, template1, template2, template3 });

    TemplateAggregate[] templates = new[] { template1, template2, template3 }
      .OrderBy(x => x.DisplayName).Skip(1).Take(2).ToArray(); // NOTE(fpion): in LINQ, null are sorted first opposed to PostgreSQL, so we eliminate '_template'.

    HashSet<Guid> ids = (await PortalContext.Templates.AsNoTracking().ToArrayAsync())
      .Select(template => new AggregateId(template.AggregateId).ToGuid()).ToHashSet();
    ids.Remove(idNotIn.Id.ToGuid());

    SearchTemplatesPayload payload = new()
    {
      Search = new TextSearch
      {
        Operator = SearchOperator.Or,
        Terms = new SearchTerm[]
        {
          new("%account%"),
          new("%create%"),
          new("%password%"),
          new("%verify%")
        }
      },
      IdIn = ids,
      ContentType = MediaTypeNames.Text.Plain,
      Realm = $" {_realm.UniqueSlug.ToUpper()} ",
      Sort = new TemplateSortOption[]
      {
        new(TemplateSort.DisplayName)
      },
      Skip = 1,
      Limit = 2
    };

    SearchResults<Template> results = await _templateService.SearchAsync(payload);

    Assert.Equal(templates.Length, results.Results.Count());
    Assert.Equal(4, results.Total);

    for (int i = 0; i < templates.Length; i++)
    {
      Assert.Equal(templates[i].Id.ToGuid(), results.Results.ElementAt(i).Id);
    }
  }

  [Fact(DisplayName = "UpdateAsync: it should return null when the template is not found.")]
  public async Task UpdateAsync_it_should_return_null_when_the_template_is_not_found()
  {
    UpdateTemplatePayload payload = new();
    Assert.Null(await _templateService.UpdateAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "UpdateAsync: it should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task UpdateAsync_it_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    TemplateAggregate template = new(_realm.UniqueNameSettings, $"{_template.UniqueName}2",
      _template.Subject, _template.ContentType, _template.Contents, _realm.Id.Value);
    await AggregateRepository.SaveAsync(template);

    UpdateTemplatePayload payload = new()
    {
      UniqueName = $" {template.UniqueName} "
    };

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<TemplateAggregate>>(
      async () => await _templateService.UpdateAsync(_template.Id.ToGuid(), payload)
    );
    Assert.Equal(template.TenantId, exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "UpdateAsync: it should update the template.")]
  public async Task UpdateAsync_it_should_update_the_template()
  {
    UpdateTemplatePayload payload = new()
    {
      DisplayName = new Modification<string>("Password Recovery"),
      Description = new Modification<string>("This is the template used for password recovery."),
      Contents = "PasswordRecovery_Body"
    };

    Template? template = await _templateService.UpdateAsync(_template.Id.ToGuid(), payload);

    Assert.NotNull(template);

    Assert.Equal(_template.Id.ToGuid(), template.Id);
    Assert.Equal(Guid.Empty, template.CreatedBy.Id);
    AssertEqual(_template.CreatedOn, template.CreatedOn);
    Assert.Equal(Actor, template.UpdatedBy);
    AssertIsNear(template.UpdatedOn);
    Assert.True(template.Version > 1);

    Assert.Equal(payload.DisplayName.Value?.Trim(), template.DisplayName);
    Assert.Equal(payload.Description.Value?.Trim(), template.Description);
    Assert.Equal(payload.Contents.Trim(), template.Contents);
  }
}
