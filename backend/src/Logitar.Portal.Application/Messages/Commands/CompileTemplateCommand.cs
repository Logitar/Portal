using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands;

public record CompileTemplateCommand(
  MessageId MessageId,
  Template Template,
  Dictionaries? Dictionaries = null,
  Locale? Locale = null,
  User? User = null,
  Variables? Variables = null) : IRequest<Content>;
