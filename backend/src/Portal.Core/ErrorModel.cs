namespace Portal.Core
{
  public class ErrorModel
  {
    public string Code { get; set; } = null!;
    public string? Description { get; set; }

    public IEnumerable<ErrorDataModel> Data { get; set; } = null!;
  }

  public class ErrorDataModel
  {
    public string Key { get; set; } = null!;
    public string? Value { get; set; }
  }
}
