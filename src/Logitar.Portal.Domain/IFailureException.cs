using FluentValidation.Results;

namespace Logitar.Portal.Domain;

public interface IFailureException
{
  ValidationFailure Failure { get; }
}
