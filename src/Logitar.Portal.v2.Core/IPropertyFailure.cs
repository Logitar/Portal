namespace Logitar.Portal.v2.Core;

public interface IPropertyFailure
{
  string ParamName { get; }
  string Value { get; }
}
