using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Dictionaries;

internal class DictionaryClientTests : IClientTests
{
  private readonly IDictionaryClient _client;

  public DictionaryClientTests(IDictionaryClient client)
  {
    _client = client;
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(_client.GetType(), nameof(_client.CreateAsync));
      CreateDictionaryPayload create = new("en");
      DictionaryModel dictionary = await _client.CreateAsync(create, context.Request);
      CreateDictionaryPayload createFr = new("fr");
      DictionaryModel dictionaryFr = await _client.CreateAsync(createFr, context.Request);
      CreateDictionaryPayload createFrCa = new("fr-CA");
      DictionaryModel dictionaryFrCa = await _client.CreateAsync(createFrCa, context.Request);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.DeleteAsync));
      dictionary = await _client.DeleteAsync(dictionary.Id, context.Request)
        ?? throw new InvalidOperationException("The dictionary should not be null.");
      dictionary = await _client.CreateAsync(create, context.Request);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReadAsync));
      DictionaryModel? notFound = await _client.ReadAsync(Guid.NewGuid(), locale: null, context.Request);
      if (notFound != null)
      {
        throw new InvalidOperationException("The dictionary should not be found.");
      }
      dictionary = await _client.ReadAsync(dictionary.Id, dictionary.Locale.Code, context.Request)
        ?? throw new InvalidOperationException("The dictionary should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.SearchAsync));
      SearchDictionariesPayload search = new()
      {
        IsEmpty = true
      };
      search.Search.Terms.Add(new SearchTerm($"%{dictionary.Locale.Code}%"));
      SearchResults<DictionaryModel> results = await _client.SearchAsync(search, context.Request);
      dictionary = results.Items.Single();
      context.Succeed();

      long version = dictionary.Version;

      context.SetName(_client.GetType(), nameof(_client.UpdateAsync));
      UpdateDictionaryPayload update = new();
      update.Entries.Add(new DictionaryEntryModification("Cordially", "Cordially,"));
      dictionary = await _client.UpdateAsync(dictionary.Id, update, context.Request)
        ?? throw new InvalidOperationException("The dictionary should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReplaceAsync));
      ReplaceDictionaryPayload replace = new(dictionary.Locale.Code);
      replace.Entries.AddRange(dictionary.Entries);
      replace.Entries.Add(new DictionaryEntry("PasswordRecovery_ClickLink", "Click on the link below to reset your password."));
      replace.Entries.Add(new DictionaryEntry("PasswordRecovery_LostYourPassword", "It seems you have lost your password..."));
      replace.Entries.Add(new DictionaryEntry("PasswordRecovery_Otherwise", "If we've been mistaken, we suggest you to delete this message."));
      replace.Entries.Add(new DictionaryEntry("PasswordRecovery_Subject", "Reset your password"));
      dictionary = await _client.ReplaceAsync(dictionary.Id, replace, version, context.Request)
        ?? throw new InvalidOperationException("The dictionary should not be null.");
      ReplaceDictionaryPayload replaceFr = new(dictionaryFr.Locale.Code);
      replaceFr.Entries.Add(new DictionaryEntry("Team", "L'équipe Logitar"));
      dictionaryFr = await _client.ReplaceAsync(dictionaryFr.Id, replaceFr, version: null, context.Request)
        ?? throw new InvalidOperationException("The dictionary should not be null.");
      ReplaceDictionaryPayload replaceFrCa = new(dictionaryFrCa.Locale.Code);
      replaceFrCa.Entries.Add(new DictionaryEntry("Hello", "Bonjour {name} !"));
      dictionaryFrCa = await _client.ReplaceAsync(dictionaryFrCa.Id, replaceFrCa, version: null, context.Request)
        ?? throw new InvalidOperationException("The dictionary should not be null.");
      context.Succeed();
    }
    catch (Exception exception)
    {
      context.Fail(exception);
    }

    return !context.HasFailed;
  }
}
