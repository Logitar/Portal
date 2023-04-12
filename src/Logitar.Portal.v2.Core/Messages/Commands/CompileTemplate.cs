using Logitar.Portal.v2.Core.Templates;
using Logitar.Portal.v2.Core.Users;
using MediatR;

namespace Logitar.Portal.v2.Core.Messages.Commands;

public record CompileTemplate(TemplateAggregate Template, Dictionaries? Dictionaries = null,
  UserAggregate? User = null, IReadOnlyDictionary<string, string>? Variables = null) : IRequest<CompiledTemplate>;
