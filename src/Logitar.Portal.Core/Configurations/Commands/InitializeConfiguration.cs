using MediatR;

namespace Logitar.Portal.Core.Configurations.Commands;

internal record InitializeConfiguration(InitializeConfigurationInput Input) : IRequest<Unit>;
