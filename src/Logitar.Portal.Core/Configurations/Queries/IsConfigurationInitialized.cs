using MediatR;

namespace Logitar.Portal.Core.Configurations.Queries;

internal record IsConfigurationInitialized : IRequest<bool>;
