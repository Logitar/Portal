using Logitar.Portal.v2.Core.Dictionaries.Events;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Handlers.Dictionaries;

internal class DictionaryDeletedHandler : INotificationHandler<DictionaryDeleted>
{
  private readonly PortalContext _context;

  public DictionaryDeletedHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(DictionaryDeleted notification, CancellationToken cancellationToken)
  {
    DictionaryEntity dictionary = await _context.Dictionaries
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The dictionary entity '{notification.AggregateId}' could not be found.");

    _context.Dictionaries.Remove(dictionary);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
