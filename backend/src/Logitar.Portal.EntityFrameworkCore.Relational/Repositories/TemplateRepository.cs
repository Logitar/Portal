using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class TemplateRepository : Repository, ITemplateRepository
{
  private readonly DbSet<TemplateEntity> _templates;

  public TemplateRepository(IEventStore eventStore, PortalContext context) : base(eventStore)
  {
    _templates = context.Templates;
  }

  public async Task<Template?> LoadAsync(TemplateId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Template?> LoadAsync(TemplateId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Template>(id.StreamId, version, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Template>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync<Template>(cancellationToken);
  }

  public async Task<IReadOnlyCollection<Template>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken)
  {
    string? tenantIdValue = tenantId?.Value;

    IEnumerable<StreamId> streamIds = (await _templates.AsNoTracking()
      .Where(x => x.TenantId == tenantIdValue)
      .Select(x => x.StreamId)
      .ToArrayAsync(cancellationToken)).Select(value => new StreamId(value));

    return await LoadAsync<Template>(streamIds, cancellationToken);
  }

  public async Task<Template?> LoadAsync(TenantId? tenantId, Identifier uniqueKey, CancellationToken cancellationToken)
  {
    string? tenantIdValue = tenantId?.Value;
    string uniqueKeyNormalized = Helper.Normalize(uniqueKey.Value);

    string? streamId = await _templates.AsNoTracking()
      .Where(x => x.TenantId == tenantIdValue && x.UniqueKeyNormalized == uniqueKeyNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : await LoadAsync<Template>(new StreamId(streamId), cancellationToken);
  }

  public async Task SaveAsync(Template template, CancellationToken cancellationToken)
  {
    await base.SaveAsync(template, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Template> templates, CancellationToken cancellationToken)
  {
    await base.SaveAsync(templates, cancellationToken);
  }
}
