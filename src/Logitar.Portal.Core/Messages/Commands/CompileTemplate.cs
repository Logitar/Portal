using Logitar.Portal.Core.Templates;
using Logitar.Portal.Core.Users;
using MediatR;

namespace Logitar.Portal.Core.Messages.Commands;

public record CompileTemplate(TemplateAggregate Template, Dictionaries? Dictionaries = null,
  UserAggregate? User = null, IReadOnlyDictionary<string, string>? Variables = null) : IRequest<CompiledTemplate>;
