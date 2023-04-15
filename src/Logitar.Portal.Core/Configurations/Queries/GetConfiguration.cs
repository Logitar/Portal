using MediatR;

namespace Logitar.Portal.Core.Configurations.Queries;

internal record GetConfiguration : IRequest<Configuration?>;
