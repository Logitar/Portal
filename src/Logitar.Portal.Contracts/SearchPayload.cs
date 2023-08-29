namespace Logitar.Portal.Contracts;

public record SearchPayload
{
  public TextSearch Search { get; set; } = new();
  public IEnumerable<Guid> IdIn { get; set; } = Enumerable.Empty<Guid>();

  public IEnumerable<SortOption> Sort { get; set; } = Enumerable.Empty<SortOption>();

  public int Skip { get; set; }
  public int Limit { get; set; }
}
