using FluentValidation;

namespace Logitar.Portal.Domain.Realms;

public record Slug
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public Slug(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Slug? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  private class Validator : AbstractValidator<Slug>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Slug(); // TODO(fpion): implement
    }
  }
}
