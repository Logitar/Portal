namespace Logitar.Portal.Contracts;

public record Modification<T> // TODO(fpion): refactor, with Identity, move to Logitar class library?
{
  public T? Value { get; set; }

  public Modification(T? value = default)
  {
    Value = value;
  }
}
