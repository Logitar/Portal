using Logitar.Portal.v2.Core.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Dictionaries.Commands;

internal record DeleteDictionaries(RealmAggregate Realm) : IRequest;
