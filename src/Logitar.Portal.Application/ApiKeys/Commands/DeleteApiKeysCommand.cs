using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal record DeleteApiKeysCommand(RealmAggregate Realm) : INotification;
