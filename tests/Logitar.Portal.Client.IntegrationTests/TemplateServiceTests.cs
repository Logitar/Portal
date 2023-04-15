using Bogus;
using Logitar.Portal.Client.Implementations;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.Client;

internal class TemplateServiceTests
{
  private readonly Faker _faker = new();

  private readonly TestContext _context;
  private readonly IRealmService _realmService;
  private readonly ITemplateService _templateService;

  public TemplateServiceTests(TestContext context, IRealmService realmService, ITemplateService templateService)
  {
    _context = context;
    _realmService = realmService;
    _templateService = templateService;
  }

  public async Task<Template?> ExecuteAsync(CancellationToken cancellationToken = default)
  {
    string name = string.Empty;
    try
    {
      string realm = _context.Realm.Id.ToString();

      name = string.Join('.', nameof(TemplateService), nameof(TemplateService.CreateAsync));
      CreateTemplateInput input = new()
      {
        Realm = realm,
        Key = "PasswordRecovery",
        Subject = "Reset your password",
        ContentType = "text/plain",
        Contents = @"@(Model.Resource(""Test""))"
      };
      Template template = await _templateService.CreateAsync(input, cancellationToken);
      await SetPasswordRecoveryTemplateAsync(template, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(TemplateService), nameof(TemplateService.UpdateAsync));
      UpdateTemplateInput update = new()
      {
        Subject = input.Subject,
        ContentType = "text/html",
        Contents = $@"<p>{input.Contents}</p>{Environment.NewLine}@(Model.Variable(""Token""))",
        DisplayName = null,
        Description = "    "
      };
      template = await _templateService.UpdateAsync(template.Id, update, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(TemplateService), nameof(TemplateService.GetAsync));
      template = (await _templateService.GetAsync(realm: realm, skip: 0, cancellationToken: cancellationToken)).Items.Single();
      _context.Succeed(name);

      name = string.Join('.', nameof(TemplateService), $"{nameof(TemplateService.GetAsync)}(id)");
      template = await _templateService.GetAsync(template.Id, cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException("The result cannot be null.");
      _context.Succeed(name);

      name = string.Join('.', nameof(TemplateService), nameof(TemplateService.DeleteAsync));
      Guid deleteId = (await _templateService.CreateAsync(new CreateTemplateInput
      {
        Realm = input.Realm,
        Key = $"{input.Key}2",
        Subject = input.Subject,
        ContentType = input.ContentType,
        Contents = input.Contents
      }, cancellationToken)).Id;
      _ = await _templateService.DeleteAsync(deleteId, cancellationToken);
      _context.Succeed(name);

      return template;
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);

      return null;
    }
  }

  private async Task SetPasswordRecoveryTemplateAsync(Template template, CancellationToken cancellationToken)
  {
    Realm realm = await _realmService.GetAsync(_context.Realm.Id, cancellationToken: cancellationToken)
      ?? throw new InvalidOperationException($"The realm '{_context.Realm.Id}' could not be found.");

    await _realmService.UpdateAsync(realm.Id, new UpdateRealmInput
    {
      DisplayName = realm.DisplayName,
      Description = realm.Description,
      DefaultLocale = realm.DefaultLocale,
      Secret = realm.Secret,
      Url = realm.Url,
      RequireConfirmedAccount = realm.RequireConfirmedAccount,
      RequireUniqueEmail = realm.RequireUniqueEmail,
      UsernameSettings = realm.UsernameSettings,
      PasswordSettings = realm.PasswordSettings,
      ClaimMappings = realm.ClaimMappings,
      CustomAttributes = realm.CustomAttributes,
      PasswordRecoverySender = realm.PasswordRecoverySender?.Id,
      PasswordRecoveryTemplate = template.Id.ToString()
    }, cancellationToken);
  }
}
