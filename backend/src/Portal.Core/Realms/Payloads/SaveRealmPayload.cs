namespace Portal.Core.Realms.Payloads
{
  public class SaveRealmPayload
  {
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
  }
}
