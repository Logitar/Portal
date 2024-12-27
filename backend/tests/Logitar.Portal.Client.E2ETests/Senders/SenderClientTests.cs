using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Senders;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.Senders;

internal class SenderClientTests : IClientTests
{
  private readonly ISenderClient _client;
  private readonly CreateSenderPayload _createPayload;

  public SenderClientTests(ISenderClient client, IConfiguration configuration)
  {
    _client = client;
    _createPayload = configuration.GetSection("Sender").Get<CreateSenderPayload>() ?? new();
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(_client.GetType(), nameof(_client.ReadDefaultAsync));
      SenderModel? @default = await _client.ReadDefaultAsync(context.Request);
      if (@default != null)
      {
        throw new InvalidOperationException("The default sender should be null.");
      }
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.CreateAsync));
      SenderModel sender = await _client.CreateAsync(_createPayload, context.Request);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.DeleteAsync));
      sender = await _client.DeleteAsync(sender.Id, context.Request)
        ?? throw new InvalidOperationException("The sender should not be null.");
      sender = await _client.CreateAsync(_createPayload, context.Request);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReadAsync));
      SenderModel? notFound = await _client.ReadAsync(Guid.NewGuid(), context.Request);
      if (notFound != null)
      {
        throw new InvalidOperationException("The sender should not be found.");
      }
      sender = await _client.ReadAsync(sender.Id, context.Request)
        ?? throw new InvalidOperationException("The sender should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.SearchAsync));
      SearchSendersPayload search = new()
      {
        Provider = sender.Provider
      };
      search.Search.Terms.Add(new SearchTerm($"{sender.EmailAddress?.Split('@').First()}%"));
      SearchResults<SenderModel> results = await _client.SearchAsync(search, context.Request);
      sender = results.Items.Single();
      context.Succeed();

      long version = sender.Version;

      context.SetName(_client.GetType(), nameof(_client.UpdateAsync));
      UpdateSenderPayload update = new()
      {
        Description = new ChangeModel<string>("This is the default sender.")
      };
      sender = await _client.UpdateAsync(sender.Id, update, context.Request)
        ?? throw new InvalidOperationException("The sender should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReplaceAsync));
      ReplaceSenderPayload replace = new(sender.EmailAddress ?? string.Empty)
      {
        DisplayName = sender.DisplayName,
        Description = sender.Description,
        Mailgun = sender.Mailgun,
        SendGrid = sender.SendGrid
      };
      sender = await _client.ReplaceAsync(sender.Id, replace, version, context.Request)
        ?? throw new InvalidOperationException("The sender should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.SetDefaultAsync));
      sender = await _client.SetDefaultAsync(sender.Id, context.Request)
        ?? throw new InvalidOperationException("The sender should not be null.");
      @default = await _client.ReadDefaultAsync(context.Request)
        ?? throw new InvalidOperationException("The default sender should not be null");
      if (@default.Id != sender.Id)
      {
        throw new InvalidOperationException($"The default sender ID '{@default.Id}' should be equal to '{sender.Id}'.");
      }
      context.Succeed();
    }
    catch (Exception exception)
    {
      context.Fail(exception);
    }

    return !context.HasFailed;
  }
}
