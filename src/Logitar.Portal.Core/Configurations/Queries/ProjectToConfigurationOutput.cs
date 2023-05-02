using MediatR;

namespace Logitar.Portal.Core.Configurations.Queries;

public record ProjectToConfigurationOutput(ConfigurationAggregate Configuration) : IRequest<Configuration>;
