namespace Portal.Core
{
  public class Error
  {
    public Error(Exception exception, IReadOnlyDictionary<string, string?>? data = null)
      : this(exception?.GetType().Name.Remove(nameof(Exception)) ?? string.Empty, exception?.Message, data)
    {
      ArgumentNullException.ThrowIfNull(exception);
    }
    public Error(string code, string? description = null, IReadOnlyDictionary<string, string?>? data = null)
    {
      Code = code ?? throw new ArgumentNullException(nameof(code));
      Description = description;
      Data = data;
    }

    public string Code { get; }
    public string? Description { get; }

    public IReadOnlyDictionary<string, string?>? Data { get; }

    public override string ToString() => Description == null ? Code : $"{Code}: {Description}";
  }
}
