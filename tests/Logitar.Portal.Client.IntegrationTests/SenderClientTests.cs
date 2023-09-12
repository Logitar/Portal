using Bogus;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Client;

internal class SenderClientTests
{
  private const string Sut = "SenderClient";

  private readonly TestContext _context;
  private readonly Faker _faker;
  private readonly ISenderService _senderService;

  public SenderClientTests(TestContext context, Faker faker, ISenderService senderService)
  {
    _context = context;
    _faker = faker;
    _senderService = senderService;
  }

  public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
  {
    string name = string.Empty;

    try
    {
      name = $"{Sut}.{nameof(_senderService.CreateAsync)}";
      CreateSenderPayload create = new()
      {
        Realm = _context.Realm.UniqueSlug,
        EmailAddress = _faker.Person.Email,
        Provider = ProviderType.SendGrid
      };
      Sender sender = await _senderService.CreateAsync(create, cancellationToken);
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_senderService.ReplaceAsync)}";
      ReplaceSenderPayload replace = new()
      {
        EmailAddress = create.EmailAddress,
        DisplayName = _faker.Person.FullName,
        Settings = new ProviderSetting[]
        {
          new("ApiKey", "SG.ABC.1234567890"),
          new("BaseUrl", "https://api.sendgrid.com/v3/mail/send")
        }
      };
      sender = await _senderService.ReplaceAsync(sender.Id, replace, sender.Version, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_senderService.UpdateAsync)}";
      UpdateSenderPayload update = new()
      {
        Description = new Modification<string>("This is the sender used per default in this realm."),
        Settings = new ProviderSettingModification[]
        {
          new("ApiKey", value: null),
          new("BaseUrl", "https://api.sendgrid.com/"),
          new("Basic", "dGVzdDpIZWxsbyBXb3JsZCE=")
        }
      };
      sender = await _senderService.UpdateAsync(sender.Id, update, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_senderService.SetDefaultAsync)}";
      Sender other = await _senderService.CreateAsync(new CreateSenderPayload
      {
        Realm = _context.Realm.Id.ToString(),
        EmailAddress = _faker.Internet.Email(),
        Provider = ProviderType.SendGrid
      }, cancellationToken);
      other = await _senderService.SetDefaultAsync(other.Id, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      sender = await _senderService.SetDefaultAsync(sender.Id, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_senderService.DeleteAsync)}";
      other = await _senderService.DeleteAsync(other.Id, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_senderService.SearchAsync)}";
      SearchSendersPayload search = new()
      {
        Realm = $"  {_context.Realm.UniqueSlug}  ",
        IdIn = new Guid[] { sender.Id }
      };
      SearchResults<Sender> results = await _senderService.SearchAsync(search, cancellationToken);
      sender = results.Results.Single();
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_senderService.ReadAsync)}:null";
      Sender? result = await _senderService.ReadAsync(Guid.Empty, cancellationToken: cancellationToken);
      if (result != null)
      {
        throw new InvalidOperationException("The realm should be null.");
      }
      _context.Succeed(name);
      name = $"{Sut}.{nameof(_senderService.ReadAsync)}:Id";
      sender = await _senderService.ReadAsync(sender.Id, cancellationToken: cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_senderService.ReadDefaultAsync)}";
      sender = await _senderService.ReadDefaultAsync(realm: _context.Realm.Id.ToString(), cancellationToken: cancellationToken)
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
