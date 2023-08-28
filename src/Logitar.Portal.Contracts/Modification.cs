namespace Logitar.Portal.Contracts;

public record Modification<T>
{
  public Modification(T? value = default)
  {
    Value = value;
  }

  public T? Value { get; }
}
