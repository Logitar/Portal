namespace Logitar.Portal.Contracts.Messages
{
  public record ResultDataModel
  {
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
  }
}
