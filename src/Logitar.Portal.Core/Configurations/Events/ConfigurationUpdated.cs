using MediatR;

namespace Logitar.Portal.Core.Configurations.Events;

public record ConfigurationUpdated : ConfigurationSaved, INotification;
