using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands
{
  internal record DeleteDictionaryCommand(string Id) : IRequest;
}
