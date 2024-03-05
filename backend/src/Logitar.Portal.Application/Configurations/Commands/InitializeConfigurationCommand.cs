using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

public record InitializeConfigurationCommand(string UniqueName, string Password) : INotification;
