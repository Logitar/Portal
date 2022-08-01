using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Web.Models.Api.Culture;
using System.Globalization;

namespace Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/locales")]
  public class LocaleApiController : ControllerBase
  {
    [HttpGet]
    public ActionResult<IEnumerable<LocaleModel>> Get()
    {
      var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
        .Where(x => x.LCID != 4096 && !string.IsNullOrEmpty(x.Name));

      return Ok(cultures.Select(culture => new LocaleModel(culture)));
    }
  }
}
