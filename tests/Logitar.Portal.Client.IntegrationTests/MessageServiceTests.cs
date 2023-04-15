using Bogus;
using Logitar.Portal.Client.Implementations;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Client;

internal class MessageServiceTests
{
  private readonly Faker _faker = new();

  private readonly TestContext _context;
  private readonly IMessageService _messageService;

  public MessageServiceTests(TestContext context, IMessageService messageService)
  {
    _context = context;
    _messageService = messageService;
  }

  public async Task<Message?> ExecuteAsync(User user, CancellationToken cancellationToken = default)
  {
    string name = string.Empty;
    try
    {
      string realm = _context.Realm.Id.ToString();

      name = string.Join('.', nameof(MessageService), nameof(MessageService.SendAsync));
      SentMessages sentMessages = await _messageService.SendAsync(new SendMessageInput
      {
        Realm = realm,
        Template = "PasswordRecovery",
        Recipients = new[]
        {
          new RecipientInput
          {
            User = user.Id.ToString()
          }
        }
      }, cancellationToken);
      Guid messageId = sentMessages.Success.Single();
      _context.Succeed(name);

      name = string.Join('.', nameof(MessageService), nameof(MessageService.GetAsync));
      Message message = (await _messageService.GetAsync(realm: realm, sort: MessageSort.UpdatedOn,
        isDescending: true, limit: 1, cancellationToken: cancellationToken)).Items.Single();
      if (message.Id != messageId)
      {
        throw new InvalidOperationException("The query did return the wrong email message.");
      }
      _context.Succeed(name);

      name = string.Join('.', nameof(MessageService), $"{nameof(MessageService.GetAsync)}(id)");
      message = await _messageService.GetAsync(messageId, cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException("The result cannot be null.");
      _context.Succeed(name);

      return message;
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);

      return null;
    }
  }
}
