using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortalDb = Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

namespace Logitar.Portal.Application.Templates.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchTemplatesQueryTests : IntegrationTests
{
  private readonly ITemplateRepository _templateRepository;

  private readonly Template _template;

  public SearchTemplatesQueryTests() : base()
  {
    _templateRepository = ServiceProvider.GetRequiredService<ITemplateRepository>();

    Identifier uniqueKey = new("PasswordRecovery");
    Subject subject = new("Reset your password");
    Content content = Content.PlainText("Hello World!");
    _template = new(uniqueKey, subject, content);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [PortalDb.Templates.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await _templateRepository.SaveAsync(_template);
  }

  [Fact(DisplayName = "It should return empty results when no template did match.")]
  public async Task It_should_return_empty_results_when_no_template_did_match()
  {
    SearchTemplatesPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("%test%"));
    SearchTemplatesQuery query = new(payload);
    SearchResults<TemplateModel> results = await ActivityPipeline.ExecuteAsync(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    Content htmlContent = Content.Html($"<p>{_template.Content.Text}</p>");

    Template notInSearch = new(new Identifier("ConfirmAccount"), new Subject("Confirm your account"), _template.Content, id: TemplateId.NewId(TenantId));
    Template notInIds = new(new Identifier($"{_template.UniqueKey.Value}_OLD"), _template.Subject, _template.Content, id: TemplateId.NewId(TenantId));
    Template html = new(new Identifier("PasswordRecoveryHtml"), _template.Subject, htmlContent, id: TemplateId.NewId(TenantId));
    Template template1 = new(_template.UniqueKey, _template.Subject, _template.Content, id: TemplateId.NewId(TenantId));
    Template template2 = new(new Identifier("ResetAccount"), new Subject("Reset your account"), _template.Content, id: TemplateId.NewId(TenantId));
    await _templateRepository.SaveAsync([notInSearch, notInIds, html, template1, template2]);

    SetRealm();

    SearchTemplatesPayload payload = new()
    {
      ContentType = MediaTypeNames.Text.Plain,
      Skip = 1,
      Limit = 1
    };
    IEnumerable<Guid> templateIds = (await _templateRepository.LoadAsync()).Select(template => template.EntityId.ToGuid());
    payload.Ids.AddRange(templateIds);
    payload.Ids.Add(Guid.NewGuid());
    payload.Ids.Remove(notInIds.EntityId.ToGuid());
    payload.Search.Terms.Add(new SearchTerm("reset%"));
    payload.Sort.Add(new TemplateSortOption(TemplateSort.Subject, isDescending: false));
    SearchTemplatesQuery query = new(payload);
    SearchResults<TemplateModel> results = await ActivityPipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    TemplateModel template = Assert.Single(results.Items);
    Assert.Equal(template1.EntityId.ToGuid(), template.Id);
  }
}
