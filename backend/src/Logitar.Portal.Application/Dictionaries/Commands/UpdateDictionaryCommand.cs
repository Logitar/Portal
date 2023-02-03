using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands
{
  internal record UpdateDictionaryCommand(string Id, UpdateDictionaryPayload Payload) : IRequest<DictionaryModel>;
}
