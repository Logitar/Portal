namespace Logitar.Portal.Contracts
{
  public record ErrorDataModel
  {
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
  }
}
