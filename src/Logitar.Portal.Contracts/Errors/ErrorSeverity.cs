namespace Logitar.Portal.Contracts.Errors;

public enum ErrorSeverity
{
  /// <summary>
  /// Errors that highlight an abnormal or unexpected event in the application flow, but do not
  /// otherwise cause the application execution to stop.
  /// </summary>
  Warning = 0,
  /// <summary>
  /// Errors that highlight when the current flow of execution is stopped due to a failure.
  /// These should indicate a failure in the current activity, not an application-wide failure.
  /// </summary>
  Failure = 1,
  /// <summary>
  /// Errors that describe an unrecoverable application or system crash, or a catastrophic
  /// failure that requires immediate attention.
  /// </summary>
  Critical = 2
}
