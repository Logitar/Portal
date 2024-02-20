using Logitar.Identity.Domain.Users;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands;

public record CompileTemplateCommand(MessageId MessageId, TemplateAggregate Template, Dictionaries? Dictionaries = null,
  UserAggregate? User = null, Variables? Variables = null) : IRequest<ContentUnit>;
