using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Passwords;
using MediatR;

namespace Logitar.Portal.Application.OneTimePasswords.Queries;

internal record ReadOneTimePasswordQuery(Guid Id) : Activity, IRequest<OneTimePasswordModel?>;
