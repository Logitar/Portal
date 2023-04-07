namespace Logitar.Portal.v2.Contracts.Users;

public record User : Aggregate
{
  public Guid Id { get; set; }

  // TODO(fpion): implement

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();
}
