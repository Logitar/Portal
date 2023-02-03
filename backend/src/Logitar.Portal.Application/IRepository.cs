using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application
{
  public interface IRepository
  {
    Task<T?> LoadAsync<T>(string id, CancellationToken cancellationToken = default) where T : AggregateRoot;
    Task<T?> LoadAsync<T>(AggregateId id, CancellationToken cancellationToken = default) where T : AggregateRoot;

    Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<AggregateId> ids, CancellationToken cancellationToken = default) where T : AggregateRoot;
    Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<string> ids, CancellationToken cancellationToken = default) where T : AggregateRoot;

    Task<Configuration?> LoadConfigurationAsync(CancellationToken cancellationToken = default);

    Task<Realm?> LoadRealmByAliasOrIdAsync(string aliasOrId, CancellationToken cancellationToken = default);

    Task<Sender?> LoadDefaultSenderAsync(AggregateId? realmId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sender>> LoadSendersByRealmAsync(Realm realm, CancellationToken cancellationToken = default);

    Task<IEnumerable<Session>> LoadActiveSessionsByUserAsync(User user, CancellationToken cancellationToken = default);
    Task<IEnumerable<Session>> LoadSessionsByRealmAsync(Realm realm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Session>> LoadSessionsByUserAsync(User user, CancellationToken cancellationToken = default);

    Task<Template?> LoadTemplateByKeyAsync(string key, Realm? realm = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Template>> LoadTemplatesByRealmAsync(Realm realm, CancellationToken cancellationToken = default);

    Task<User?> LoadUserByExternalProviderAsync(Realm realm, string key, string value, CancellationToken cancellationToken = default);
    Task<User?> LoadUserByUsernameAsync(string username, Realm? realm = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> LoadUsersByEmailAsync(string email, Realm? realm = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> LoadUsersByRealmAsync(Realm realm, CancellationToken cancellationToken = default);

    Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken = default);
    Task SaveAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken = default);
  }
}
