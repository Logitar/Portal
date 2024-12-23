using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Activities;

public record ActivityContext(ConfigurationModel Configuration, Realm? Realm, ApiKeyModel? ApiKey, User? User, Session? Session);
