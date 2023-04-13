using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Queries;

internal record GetUser(Guid? Id, string? Realm, string? Username,
  string? ExternalKey, string? ExternalValue) : IRequest<User?>;
