using Logitar.Portal.Client.Implementations;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Client;

internal class DictionaryServiceTests
{
  private readonly TestContext _context;
  private readonly IDictionaryService _dictionaryService;

  public DictionaryServiceTests(TestContext context, IDictionaryService dictionaryService)
  {
    _context = context;
    _dictionaryService = dictionaryService;
  }

  public async Task<Dictionary?> ExecuteAsync(CancellationToken cancellationToken = default)
  {
    string name = string.Empty;
    try
    {
      string realm = _context.Realm.Id.ToString();
      string locale = _context.Realm.DefaultLocale ?? string.Empty;

      name = string.Join('.', nameof(DictionaryService), nameof(DictionaryService.CreateAsync));
      CreateDictionaryInput input = new()
      {
        Realm = realm,
        Locale = locale
      };
      Dictionary dictionary = await _dictionaryService.CreateAsync(input, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(DictionaryService), nameof(DictionaryService.UpdateAsync));
      UpdateDictionaryInput update = new()
      {
        Entries = new[]
        {
          new Entry
          {
            Key = "Test",
            Value = "Hello World!"
          }
        }
      };
      dictionary = await _dictionaryService.UpdateAsync(dictionary.Id, update, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(DictionaryService), nameof(DictionaryService.GetAsync));
      dictionary = (await _dictionaryService.GetAsync(realm: realm, cancellationToken: cancellationToken)).Items.Single();
      _context.Succeed(name);

      name = string.Join('.', nameof(DictionaryService), $"{nameof(DictionaryService.GetAsync)}(id)");
      dictionary = await _dictionaryService.GetAsync(dictionary.Id, cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException("The result cannot be null.");
      _context.Succeed(name);

      name = string.Join('.', nameof(DictionaryService), nameof(DictionaryService.DeleteAsync));
      Guid deleteId = (await _dictionaryService.CreateAsync(new CreateDictionaryInput
      {
        Realm = realm,
        Locale = locale == "fr-CA" ? "fr-FR" : "fr-CA"
      }, cancellationToken)).Id;
      _ = await _dictionaryService.DeleteAsync(deleteId, cancellationToken);
      _context.Succeed(name);

      return dictionary;
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);

      return null;
    }
  }
}
