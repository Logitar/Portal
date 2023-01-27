using FluentValidation;
using System.Globalization;

namespace Logitar.Portal.Core
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
    public static IRuleBuilderOptions<T, CultureInfo?> Locale<T>(this IRuleBuilder<T, CultureInfo?> rules)
    {
      return rules.Must(culture => culture == null || culture.LCID != 4096);
    }
    /// <summary>
    /// TODO(fpion): WithErrorCode?
    /// TODO(fpion): WithMessage?
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rules"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string?> NullOrNotEmpty<T>(this IRuleBuilder<T, string?> rules)
    {
      return rules.Must(s => s == null || !string.IsNullOrWhiteSpace(s));
    }
    /// <summary>
    /// TODO(fpion): WithErrorCode?
    /// TODO(fpion): WithMessage?
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rules"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string?> Uri<T>(this IRuleBuilderOptions<T, string?> rules)
    {
      return rules.Must(u => u == null || System.Uri.IsWellFormedUriString(u, UriKind.RelativeOrAbsolute));
    }
  }
}
