using FluentValidation;
using Logitar.Portal.Domain.Realms.Validators;

namespace Logitar.Portal.Domain.Realms;

public record ReadOnlyClaimMapping
{
  public ReadOnlyClaimMapping(string name, string? type = null)
  {
    Name = name.Trim();
    Type = type?.CleanTrim();

    new ReadOnlyClaimMappingValidator().ValidateAndThrow(this);
  }

  public string Name { get; }
  public string? Type { get; }
}
