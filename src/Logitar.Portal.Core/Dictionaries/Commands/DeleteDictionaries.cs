using Logitar.Portal.Core.Realms;
using MediatR;

namespace Logitar.Portal.Core.Dictionaries.Commands;

internal record DeleteDictionaries(RealmAggregate Realm) : IRequest;
