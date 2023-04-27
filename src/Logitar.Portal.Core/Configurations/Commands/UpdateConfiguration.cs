using MediatR;

namespace Logitar.Portal.Core.Configurations.Commands;

internal record UpdateConfiguration(UpdateConfigurationInput Input) : IRequest<Configuration>;
