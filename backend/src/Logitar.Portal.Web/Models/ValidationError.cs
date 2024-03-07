using FluentValidation;
using FluentValidation.Results;
using Logitar.Portal.Contracts.Errors;

namespace Logitar.Portal.Web.Models;

public record ValidationError : Error
{
  public List<ValidationFailure> Errors { get; set; }

  public ValidationError() : this(string.Empty, string.Empty)
  {
  }

  public ValidationError(ValidationException exception) : this(exception.GetErrorCode(), exception.Message, exception.Errors)
  {
  }

  public ValidationError(string code, string message) : base(code, message)
  {
    Errors = [];
  }

  public ValidationError(string code, string message, IEnumerable<ValidationFailure> errors) : this(code, message)
  {
    Errors.AddRange(errors);
  }
}
