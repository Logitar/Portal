using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal record DeleteApiKeyCommand(Guid Id) : Activity,IRequest<ApiKey?>;
