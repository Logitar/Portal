using FluentValidation;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Domain.Templates;

public record UniqueKey // ISSUE #528: replace with Identifier
{
  public string Value { get; }

  public UniqueKey(string value)
  {
    Value = value.Trim();
    new IdentifierValidator().ValidateAndThrow(Value);
  }

  public static UniqueKey? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;
}
