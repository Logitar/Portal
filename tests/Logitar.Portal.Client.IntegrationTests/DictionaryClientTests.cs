using Bogus;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Client;

internal class DictionaryClientTests
{
  private const string Sut = "DictionaryClient";

  private readonly TestContext _context;
  private readonly IDictionaryService _dictionaryService;
  private readonly Faker _faker;

  public DictionaryClientTests(TestContext context, Faker faker, IDictionaryService dictionaryService)
  {
    _context = context;
    _faker = faker;
    _dictionaryService = dictionaryService;
  }

  public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
  {
    string name = string.Empty;

    try
    {
      name = $"{Sut}.{nameof(_dictionaryService.CreateAsync)}";
      CreateDictionaryPayload create = new()
      {
        Realm = _context.Realm.UniqueSlug,
        Locale = "fr",
        Entries = new DictionaryEntry[]
        {
          new("Red", "Rouge"),
          new("Green", "Vert"),
          new("Blue", "Bleu")
        }
      };
      Dictionary dictionary = await _dictionaryService.CreateAsync(create, cancellationToken);
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_dictionaryService.ReplaceAsync)}";
      ReplaceDictionaryPayload replace = new()
      {
        Entries = new DictionaryEntry[]
        {
          new("Red", "Rouge"),
          new("Blue", "Cyan"),
          new("Yellow", "Jaune")
        }
      };
      dictionary = await _dictionaryService.ReplaceAsync(dictionary.Id, replace, dictionary.Version, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_dictionaryService.UpdateAsync)}";
      UpdateDictionaryPayload update = new()
      {
        Entries = new DictionaryEntryModification[]
        {
          new("PasswordRecovery_Body", "Votre code d’accès afin de réinitialiser votre mot de passe est : {code}."),
          new("PasswordRecovery_Subject", "Réinitialiser votre mot de passe"),
          new("White", "Blanc"),
          new("Blue", "Bleu"),
          new("Yellow", value: null),
        }
      };
      dictionary = await _dictionaryService.UpdateAsync(dictionary.Id, update, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_dictionaryService.DeleteAsync)}";
      Dictionary delete = await _dictionaryService.CreateAsync(new CreateDictionaryPayload
      {
        Realm = _context.Realm.Id.ToString(),
        Locale = "es"
      }, cancellationToken);
      delete = await _dictionaryService.DeleteAsync(delete.Id, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_dictionaryService.SearchAsync)}";
      SearchDictionariesPayload search = new()
      {
        Realm = $"  {_context.Realm.UniqueSlug}  ",
        IdIn = new Guid[] { dictionary.Id }
      };
      SearchResults<Dictionary> results = await _dictionaryService.SearchAsync(search, cancellationToken);
      dictionary = results.Results.Single();
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_dictionaryService.ReadAsync)}:null";
      Dictionary? result = await _dictionaryService.ReadAsync(Guid.Empty, cancellationToken: cancellationToken);
      if (result != null)
      {
        throw new InvalidOperationException("The realm should be null.");
      }
      _context.Succeed(name);
      name = $"{Sut}.{nameof(_dictionaryService.ReadAsync)}:Id";
      dictionary = await _dictionaryService.ReadAsync(dictionary.Id, cancellationToken: cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);
      name = $"{Sut}.{nameof(_dictionaryService.ReadAsync)}:Locale";
      dictionary = await _dictionaryService.ReadAsync(realm: _context.Realm.Id.ToString(), locale: dictionary.Locale.Code, cancellationToken: cancellationToken)
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
