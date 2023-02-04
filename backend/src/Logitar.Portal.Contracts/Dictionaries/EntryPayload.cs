namespace Logitar.Portal.Contracts.Dictionaries
{
  public record EntryPayload
  {
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
  }
}
