using FluentValidation;
using FluentValidation.Results;
using Logitar.Portal.Contracts.Errors;

namespace Logitar.Portal.Application.Errors;

public record ValidationError : Error
{
  public IEnumerable<ValidationFailure> Errors { get; set; }

  public ValidationError() : this(string.Empty, string.Empty, [])
  {
  }

  public ValidationError(ValidationException exception) : base(exception.GetErrorCode(), $"Validation failed. See {nameof(Errors)} for more information.")
  {
    Errors = exception.Errors;
  }

  public ValidationError(string code, string message, IEnumerable<ValidationFailure> errors) : base(code, message)
  {
    Errors = errors;
  }
}
