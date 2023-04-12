using MediatR;

namespace Logitar.Portal.v2.Core.Configurations.Queries;

internal record IsConfigurationInitialized : IRequest<bool>;
