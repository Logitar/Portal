using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal record DeleteDictionariesCommand(RealmAggregate Realm) : INotification;
