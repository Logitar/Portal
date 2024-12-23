using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries;

internal record FindUserQuery(TenantId? TenantId, string User, IUserSettings UserSettings, string? PropertyName, bool IncludeId = false) : IRequest<User>;
