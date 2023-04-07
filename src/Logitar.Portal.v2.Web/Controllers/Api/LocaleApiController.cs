using Logitar.Portal.v2.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logitar.Portal.v2.Web.Controllers.Api;

[ApiController]
//[Authorize(Policy = Constants.Policies.PortalIdentity)] // TODO(fpion): Authorization
[Route("api/locales")]
public class LocaleApiController : ControllerBase
{
  private static readonly IEnumerable<Locale> _locales = CultureInfo.GetCultures(CultureTypes.AllCultures)
    .Where(x => x.LCID != 4096 && !string.IsNullOrEmpty(x.Name))
    .Select(culture => new Locale(culture));

  [HttpGet]
  public ActionResult<IEnumerable<Locale>> Get() => Ok(_locales);
}
