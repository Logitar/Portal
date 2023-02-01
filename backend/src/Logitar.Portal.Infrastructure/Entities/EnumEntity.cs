namespace Logitar.Portal.Infrastructure.Entities
{
  internal abstract class EnumEntity<T> where T : new()
  {
    protected EnumEntity()
    {
    }
    protected EnumEntity(T value, string name)
    {
      Value = value;
      Name = name;
    }

    public T Value { get; protected set; } = new();
    public string Name { get; protected set; } = string.Empty;
  }
}
