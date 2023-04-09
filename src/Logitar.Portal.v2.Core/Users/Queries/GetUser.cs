using Logitar.Portal.v2.Contracts.Users;
using MediatR;

namespace Logitar.Portal.v2.Core.Users.Queries;

internal record GetUser(Guid? Id, string? Realm, string? Username,
  string? ExternalKey, string? ExternalValue) : IRequest<User?>;
