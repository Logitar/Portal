using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands;

public record CompileTemplateCommand(MessageId MessageId, Template Template, Dictionaries? Dictionaries = null, LocaleUnit? Locale = null,
  UserAggregate? User = null, Variables? Variables = null) : IRequest<Content>;
