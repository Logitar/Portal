using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Messages;

internal class MessageClientTests : IClientTests
{
  private readonly IMessageClient _client;

  public MessageClientTests(IMessageClient client)
  {
    _client = client;
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(_client.GetType(), nameof(_client.SendAsync));
      SendMessagePayload send = new(context.Template.UniqueKey)
      {
        IsDemo = true
      };
      send.Recipients.Add(new RecipientPayload
      {
        UserId = context.User.Id
      });
      send.Variables.Add(new Variable("Token", context.Token));
      SentMessages sentMessages = await _client.SendAsync(send, context.Request);
      Guid messageId = sentMessages.Ids.Single();
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReadAsync));
      Message? notFound = await _client.ReadAsync(Guid.NewGuid(), context.Request);
      if (notFound != null)
      {
        throw new InvalidOperationException("The message should not be found.");
      }
      Message message = await _client.ReadAsync(messageId, context.Request)
        ?? throw new InvalidOperationException("The message should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.SearchAsync));
      SearchMessagesPayload payload = new()
      {
        TemplateId = context.Template.Id,
        IsDemo = true,
        Status = MessageStatus.Succeeded
      };
      payload.Search.Terms.Add(new SearchTerm("Reset your password"));
      context.Succeed();
    }
    catch (Exception exception)
    {
      context.Fail(exception);
    }

    return !context.HasFailed;
  }
}
