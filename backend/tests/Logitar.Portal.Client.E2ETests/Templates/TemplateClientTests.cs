using Logitar.Identity.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.Templates;

internal class TemplateClientTests : IClientTests
{
  private readonly ITemplateClient _client;

  public TemplateClientTests(ITemplateClient client)
  {
    _client = client;
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(_client.GetType(), nameof(_client.CreateAsync));
      string text = await File.ReadAllTextAsync("Templates/PasswordRecovery.html");
      Content content = Content.Html(text);
      CreateTemplatePayload create = new("PasswordRecovery", "PasswordRecovery_Subject", content);
      Template template = await _client.CreateAsync(create, context.Request);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.DeleteAsync));
      template = await _client.DeleteAsync(template.Id, context.Request)
        ?? throw new InvalidOperationException("The template should not be null.");
      template = await _client.CreateAsync(create, context.Request);
      context.SetTemplate(template);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReadAsync));
      Template? notFound = await _client.ReadAsync(Guid.NewGuid(), uniqueKey: null, context.Request);
      if (notFound != null)
      {
        throw new InvalidOperationException("The template should not be found.");
      }
      template = await _client.ReadAsync(template.Id, template.UniqueKey, context.Request)
        ?? throw new InvalidOperationException("The template should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.SearchAsync));
      SearchTemplatesPayload search = new()
      {
        ContentType = MediaTypeNames.Text.Html
      };
      search.Search.Terms.Add(new SearchTerm("%_Subject"));
      SearchResults<Template> results = await _client.SearchAsync(search, context.Request);
      template = results.Items.Single();
      context.Succeed();

      long version = template.Version;

      context.SetName(_client.GetType(), nameof(_client.UpdateAsync));
      UpdateTemplatePayload update = new()
      {
        DisplayName = new Modification<string>("Password Recovery")
      };
      template = await _client.UpdateAsync(template.Id, update, context.Request)
        ?? throw new InvalidOperationException("The template should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReplaceAsync));
      ReplaceTemplatePayload replace = new(template.UniqueKey, template.Subject, template.Content)
      {
        DisplayName = null,
        Description = template.Description
      };
      template = await _client.ReplaceAsync(template.Id, replace, version, context.Request)
        ?? throw new InvalidOperationException("The template should not be null.");
      context.Succeed();
    }
    catch (Exception exception)
    {
      context.Fail(exception);
    }

    return !context.HasFailed;
  }
}
