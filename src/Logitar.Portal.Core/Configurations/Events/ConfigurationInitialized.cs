using MediatR;

namespace Logitar.Portal.Core.Configurations.Events;

public record ConfigurationInitialized : ConfigurationSaved, INotification;
