namespace Logitar.Portal.Contracts.Errors;

public record Error
{
  public string Code { get; set; }
  public string Message { get; set; }
  public List<ErrorData> Data { get; set; }

  public Error() : this(string.Empty, string.Empty)
  {
  }

  public Error(string code, string message)
  {
    Code = code;
    Message = message;
    Data = [];
  }

  public Error(string code, string message, IEnumerable<ErrorData> data) : this(code, message)
  {
    Data.AddRange(data);
  }

  public void AddData(KeyValuePair<string, string?> pair) => AddData(pair.Key, pair.Value);
  public void AddData(string key, string? value) => AddData(new ErrorData(key, value));
  public void AddData(ErrorData data) => Data.Add(data);
}
