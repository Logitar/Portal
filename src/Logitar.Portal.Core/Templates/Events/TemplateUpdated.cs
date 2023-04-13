using MediatR;

namespace Logitar.Portal.Core.Templates.Events;

public record TemplateUpdated : TemplateSaved, INotification;
