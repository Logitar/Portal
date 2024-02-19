using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Templates.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchTemplatesQueryTests : IntegrationTests
{
  private readonly ITemplateRepository _templateRepository;

  private readonly TemplateAggregate _template;

  public SearchTemplatesQueryTests() : base()
  {
    _templateRepository = ServiceProvider.GetRequiredService<ITemplateRepository>();

    UniqueKeyUnit uniqueKey = new("PasswordRecovery");
    SubjectUnit subject = new("Reset your password");
    ContentUnit content = ContentUnit.PlainText("Hello World!");
    _template = new(uniqueKey, subject, content);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [PortalDb.Templates.Table];
    foreach (TableId table in tables)
    {
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
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
    SearchResults<Template> results = await Mediator.Send(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    ContentUnit htmlContent = ContentUnit.Html($"<p>{_template.Content.Text}</p>");

    TemplateAggregate notInSearch = new(new UniqueKeyUnit("ConfirmAccount"), new SubjectUnit("Confirm your account"), _template.Content, TenantId);
    TemplateAggregate notInIds = new(new UniqueKeyUnit($"{_template.UniqueKey.Value}_OLD"), _template.Subject, _template.Content, TenantId);
    TemplateAggregate html = new(new UniqueKeyUnit("PasswordRecoveryHtml"), _template.Subject, htmlContent, TenantId);
    TemplateAggregate template1 = new(_template.UniqueKey, _template.Subject, _template.Content, TenantId);
    TemplateAggregate template2 = new(new UniqueKeyUnit("ResetAccount"), new SubjectUnit("Reset your account"), _template.Content, TenantId);
    await _templateRepository.SaveAsync([notInSearch, notInIds, html, template1, template2]);

    SetRealm();

    SearchTemplatesPayload payload = new()
    {
      ContentType = MediaTypeNames.Text.Plain,
      Skip = 1,
      Limit = 1
    };
    IEnumerable<Guid> templateIds = (await _templateRepository.LoadAsync()).Select(template => template.Id.AggregateId.ToGuid());
    payload.Ids.AddRange(templateIds);
    payload.Ids.Add(Guid.NewGuid());
    payload.Ids.Remove(notInIds.Id.AggregateId.ToGuid());
    payload.Search.Terms.Add(new SearchTerm("reset%"));
    payload.Sort.Add(new TemplateSortOption(TemplateSort.Subject, isDescending: false));
    SearchTemplatesQuery query = new(payload);
    SearchResults<Template> results = await Mediator.Send(query);

    Assert.Equal(2, results.Total);
    Template template = Assert.Single(results.Items);
    Assert.Equal(template1.Id.AggregateId.ToGuid(), template.Id);
  }
}
