using FluentValidation;
using System.Globalization;

namespace Logitar.Portal.Application
{
  public static class FluentValidationExtensions
  {
    /// <summary>
    /// TODO(fpion): WithErrorCode?
    /// TODO(fpion): WithMessage?
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rules"></param>
    /// <returns></returns>
    public static IRuleBuilder<T, CultureInfo?> Locale<T>(this IRuleBuilder<T, CultureInfo?> rules)
    {
      return rules.Must(c => c == null || c.LCID != 4096);
    }
    /// <summary>
    /// TODO(fpion): WithErrorCode?
    /// TODO(fpion): WithMessage?
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rules"></param>
    /// <returns></returns>
    public static IRuleBuilder<T, string?> NullOrNotEmpty<T>(this IRuleBuilder<T, string?> rules)
    {
      return rules.Must(s => s == null || !string.IsNullOrWhiteSpace(s));
    }
  }
}
