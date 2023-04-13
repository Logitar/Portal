using Logitar.Portal.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using System.Globalization;

namespace Logitar.Portal.Web.Controllers.Api;

[ApiController]
[Authorize(Policy = Constants.Policies.PortalActor)]
[Route("api")]
public class ResourceApiController : ControllerBase
{
  private static readonly IEnumerable<Locale> _locales = CultureInfo.GetCultures(CultureTypes.AllCultures)
    .Where(x => x.LCID != 4096 && !string.IsNullOrEmpty(x.Name))
    .Select(culture => new Locale(culture));
  private static readonly IEnumerable<TimeZoneEntry> _timeZones = DateTimeZoneProviders.Tzdb.Ids
    .Select(id => new TimeZoneEntry(id));

  [HttpGet("locales")]
  public ActionResult<IEnumerable<Locale>> GetLocales() => Ok(_locales);

  [HttpGet("timezones")]
  public ActionResult<IEnumerable<TimeZoneEntry>> GetTimeZones() => Ok(_timeZones);
}
