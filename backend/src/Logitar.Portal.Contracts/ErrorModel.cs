namespace Logitar.Portal.Contracts
{
  public record ErrorModel
  {
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    public IEnumerable<ErrorDataModel> Data { get; set; } = Enumerable.Empty<ErrorDataModel>();
  }
}
