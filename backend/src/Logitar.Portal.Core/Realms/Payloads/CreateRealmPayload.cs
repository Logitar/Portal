namespace Logitar.Portal.Core.Realms.Payloads
{
  public class CreateRealmPayload : SaveRealmPayload
  {
    public string Alias { get; set; } = null!;
  }
}
