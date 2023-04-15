using Bogus;
using Logitar.Portal.Client.Implementations;
using Logitar.Portal.Contracts.Senders;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.Client;

internal class SenderServiceTests
{
  private readonly Faker _faker = new();

  private readonly TestContext _context;
  private readonly ISenderService _senderService;
  private readonly SenderSettings _senderSettings;
  private readonly SendGridSettings _sendGridSettings;

  public SenderServiceTests(IConfiguration configuration, TestContext context, ISenderService senderService)
  {
    _context = context;
    _senderService = senderService;
    _senderSettings = configuration.GetSection("Sender").Get<SenderSettings>() ?? new();
    _sendGridSettings = configuration.GetSection("SendGrid").Get<SendGridSettings>() ?? new();
  }

  public async Task<Sender?> ExecuteAsync(CancellationToken cancellationToken = default)
  {
    string name = string.Empty;
    try
    {
      string realm = _context.Realm.Id.ToString();
      string locale = _context.Realm.DefaultLocale ?? string.Empty;

      name = string.Join('.', nameof(SenderService), nameof(SenderService.CreateAsync));
      CreateSenderInput input = new()
      {
        Realm = realm,
        Provider = ProviderType.SendGrid,
        EmailAddress = _faker.Person.Email,
        Settings = new[]
        {
          new Setting
          {
            Key = "ApiKey",
            Value = _sendGridSettings.ApiKey
          }
        }
      };
      Sender sender = await _senderService.CreateAsync(input, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(SenderService), nameof(SenderService.UpdateAsync));
      UpdateSenderInput update = new()
      {
        EmailAddress = _senderSettings.EmailAddress,
        DisplayName = _senderSettings.DisplayName,
        Settings = input.Settings
      };
      sender = await _senderService.UpdateAsync(sender.Id, update, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(SenderService), nameof(SenderService.GetAsync));
      sender = (await _senderService.GetAsync(realm: realm, cancellationToken: cancellationToken)).Items.Single();
      _context.Succeed(name);

      name = string.Join('.', nameof(SenderService), $"{nameof(SenderService.GetAsync)}(id)");
      sender = await _senderService.GetAsync(sender.Id, cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException("The result cannot be null.");
      _context.Succeed(name);

      name = string.Join('.', nameof(SenderService), nameof(SenderService.SetDefaultAsync));
      sender = await _senderService.SetDefaultAsync(sender.Id, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(SenderService), nameof(SenderService.DeleteAsync));
      Guid deleteId = (await _senderService.CreateAsync(input, cancellationToken)).Id;
      _ = await _senderService.DeleteAsync(deleteId, cancellationToken);
      _context.Succeed(name);

      return sender;
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);

      return null;
    }
  }
}
