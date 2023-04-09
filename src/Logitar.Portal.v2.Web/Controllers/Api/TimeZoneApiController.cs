using Logitar.Portal.v2.Web.Models;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace Logitar.Portal.v2.Web.Controllers.Api;

[ApiController]
//[Authorize(Policy = Constants.Policies.PortalIdentity)] // TODO(fpion): Authorization
[Route("api/timezones")]
public class TimeZoneApiController : ControllerBase
{
  private static readonly IEnumerable<TimeZoneEntry> _timeZones = DateTimeZoneProviders.Tzdb.Ids
    .Select(id => new TimeZoneEntry(id));

  [HttpGet]
  public ActionResult<IEnumerable<TimeZoneEntry>> Get() => Ok(_timeZones);
}
