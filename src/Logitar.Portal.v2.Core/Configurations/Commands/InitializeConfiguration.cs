using MediatR;

namespace Logitar.Portal.v2.Core.Configurations.Commands;

internal record InitializeConfiguration(InitializeConfigurationInput Input, Uri? Url) : IRequest;
