namespace Logitar.Portal.Contracts;

public record Modification<T> // TODO(fpion): refactor
{
  public T? Value { get; set; }

  public Modification(T? value = default)
  {
    Value = value;
  }
}
