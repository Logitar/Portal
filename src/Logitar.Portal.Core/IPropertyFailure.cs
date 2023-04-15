namespace Logitar.Portal.Core;

public interface IPropertyFailure
{
  string ParamName { get; }
  string Value { get; }
}
