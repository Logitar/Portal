using Logitar.Portal.Contracts.Passwords;
using MediatR;

namespace Logitar.Portal.Application.OneTimePasswords.Queries;

internal record ReadOneTimePasswordQuery(Guid Id) : ApplicationRequest, IRequest<OneTimePassword?>;
