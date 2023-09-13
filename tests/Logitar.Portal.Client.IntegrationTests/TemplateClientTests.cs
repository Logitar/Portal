using Bogus;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.Client;

internal class TemplateClientTests
{
  private const string Sut = "TemplateClient";

  private readonly TestContext _context;
  private readonly Faker _faker;
  private readonly ITemplateService _templateService;

  public TemplateClientTests(TestContext context, Faker faker, ITemplateService templateService)
  {
    _context = context;
    _faker = faker;
    _templateService = templateService;
  }

  public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
  {
    string name = string.Empty;

    try
    {
      name = $"{Sut}.{nameof(_templateService.CreateAsync)}";
      CreateTemplatePayload create = new()
      {
        Realm = _context.Realm.UniqueSlug,
        UniqueName = "PasswordRecovery",
        Subject = "PasswordRecovery_Subject",
        ContentType = MediaTypeNames.Text.Plain,
        Contents = "Hello World!"
      };
      Template template = await _templateService.CreateAsync(create, cancellationToken);
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_templateService.ReplaceAsync)}";
      ReplaceTemplatePayload replace = new()
      {
        UniqueName = "PasswordRecovery",
        DisplayName = "Password Recovery",
        Description = "This is the template used for password recovery.",
        Subject = "PasswordRecovery_Subject",
        ContentType = MediaTypeNames.Text.Plain,
        Contents = "PasswordRecovery_Body"
      };
      template = await _templateService.ReplaceAsync(template.Id, replace, template.Version, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_templateService.UpdateAsync)}";
      UpdateTemplatePayload update = new()
      {
        Description = new Modification<string>(null)
      };
      template = await _templateService.UpdateAsync(template.Id, update, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_templateService.DeleteAsync)}";
      Template other = await _templateService.CreateAsync(new CreateTemplatePayload
      {
        Realm = _context.Realm.Id.ToString(),
        UniqueName = "CreateUser",
        Subject = "CreateUser_Subject",
        ContentType = MediaTypeNames.Text.Plain,
        Contents = "CreateUser_Body"
      }, cancellationToken);
      other = await _templateService.DeleteAsync(other.Id, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_templateService.SearchAsync)}";
      SearchTemplatesPayload search = new()
      {
        Realm = $"  {_context.Realm.UniqueSlug}  ",
        IdIn = new Guid[] { template.Id }
      };
      SearchResults<Template> results = await _templateService.SearchAsync(search, cancellationToken);
      template = results.Results.Single();
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_templateService.ReadAsync)}:null";
      Template? result = await _templateService.ReadAsync(Guid.Empty, cancellationToken: cancellationToken);
      if (result != null)
      {
        throw new InvalidOperationException("The realm should be null.");
      }
      _context.Succeed(name);
      name = $"{Sut}.{nameof(_templateService.ReadAsync)}:Id";
      template = await _templateService.ReadAsync(template.Id, cancellationToken: cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);
      name = $"{Sut}.{nameof(_templateService.ReadAsync)}:UniqueName";
      template = await _templateService.ReadAsync(realm: _context.Realm.Id.ToString(), uniqueName: template.UniqueName, cancellationToken: cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);
    }

    return !_context.HasFailed;
  }
}
