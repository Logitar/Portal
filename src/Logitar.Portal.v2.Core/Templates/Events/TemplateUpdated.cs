using MediatR;

namespace Logitar.Portal.v2.Core.Templates.Events;

public record TemplateUpdated : TemplateSaved, INotification;
