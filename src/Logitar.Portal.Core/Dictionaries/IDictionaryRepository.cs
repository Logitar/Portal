using Logitar.Portal.Core.Realms;
using System.Globalization;

namespace Logitar.Portal.Core.Dictionaries;

public interface IDictionaryRepository
{
  Task<DictionaryAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IEnumerable<DictionaryAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken = default);
  Task<DictionaryAggregate?> LoadAsync(RealmAggregate? realm, CultureInfo locale, CancellationToken cancellationToken = default);
  Task SaveAsync(DictionaryAggregate dictionary, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<DictionaryAggregate> dictionaries, CancellationToken cancellationToken = default);
}
