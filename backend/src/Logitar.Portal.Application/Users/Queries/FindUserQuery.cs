using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries;

internal record FindUserQuery(TenantId? TenantId, string User, IUserSettings UserSettings, string? PropertyName, bool IncludeId = false) : IRequest<UserAggregate>;
