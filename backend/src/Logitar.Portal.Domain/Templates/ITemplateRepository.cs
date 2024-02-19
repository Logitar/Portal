namespace Logitar.Portal.Domain.Templates;

public interface ITemplateRepository
{
  Task<IEnumerable<TemplateAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task<TemplateAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<TemplateAggregate?> LoadAsync(TemplateId id, long? version, CancellationToken cancellationToken = default);
  Task<TemplateAggregate?> LoadAsync(UniqueKeyUnit uniqueKey, CancellationToken cancellationToken = default);

  Task SaveAsync(TemplateAggregate template, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<TemplateAggregate> templates, CancellationToken cancellationToken = default);
}
