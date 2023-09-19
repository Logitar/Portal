using Bogus;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.Client;

internal class MessageClientTests
{
  private const string RecipientKey = "Recipient";
  private const string Sut = "MessageClient";

  private readonly TestContext _context;
  private readonly Faker _faker;
  private readonly MessageClient _messageClient;
  private readonly IMessageService _messageService;
  private readonly string _recipient;

  public MessageClientTests(IConfiguration configuration, TestContext context, Faker faker, MessageClient messageClient, IMessageService messageService)
  {
    _context = context;
    _faker = faker;
    _messageService = messageService;
    _messageClient = messageClient;
    _recipient = configuration.GetValue<string>(RecipientKey) ?? throw new InvalidOperationException($"The configuration '{RecipientKey}' is required.");
  }

  public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
  {
    string name = string.Empty;

    try
    {
      name = $"{Sut}.{nameof(_messageService.SendAsync)}";
      SendMessagePayload send = new()
      {
        Realm = _context.Realm.UniqueSlug,
        Template = "PasswordRecovery",
        Recipients = new RecipientPayload[]
        {
          new()
          {
            Address = _recipient,
            DisplayName = _context.User.FullName
          }
        },
        IgnoreUserLocale = true,
        Locale = "fr",
        Variables = new Variable[]
        {
          new("Token", Guid.NewGuid().ToString())
        }
      };
      SentMessages sentMessages = await _messageService.SendAsync(send, cancellationToken);
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_messageService.SearchAsync)}";
      SearchMessagesPayload search = new()
      {
        Realm = $"  {_context.Realm.UniqueSlug}  ",
        IdIn = sentMessages.Ids
      };
      SearchResults<Message> results = await _messageService.SearchAsync(search, cancellationToken);
      Message message = results.Results.Single();
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_messageService.ReadAsync)}:null";
      Message? result = await _messageService.ReadAsync(Guid.Empty, cancellationToken: cancellationToken);
      if (result != null)
      {
        throw new InvalidOperationException("The realm should be null.");
      }
      _context.Succeed(name);
      name = $"{Sut}.{nameof(_messageService.ReadAsync)}:Id";
      message = await _messageService.ReadAsync(message.Id, cancellationToken: cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_messageService.SendDemoAsync)}";
      SendDemoMessagePayload payload = new()
      {
        TemplateId = _context.Template.Id,
        Locale = "fr-CA",
        Variables = new Variable[]
      {
          new("Token", Guid.NewGuid().ToString())
      }
      };
      _ = await _messageClient.SendDemoAsync(payload, cancellationToken);
      _context.Succeed(name);
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);
    }

    return !_context.HasFailed;
  }
}
