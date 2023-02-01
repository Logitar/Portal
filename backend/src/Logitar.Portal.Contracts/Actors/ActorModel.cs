namespace Logitar.Portal.Contracts.Actors
{
  public class ActorModel
  {
    public string Id { get; set; } = string.Empty;
    public ActorType Type { get; set; }
    public bool IsDeleted { get; set; }

    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Picture { get; set; }
  }
}
