using Logitar.Portal.Web.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logitar.Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/locales")]
  public class LocaleApiController : ControllerBase
  {
    private static readonly IEnumerable<LocaleModel> _locales = CultureInfo.GetCultures(CultureTypes.AllCultures)
      .Where(x => !string.IsNullOrEmpty(x.Name) && x.LCID != 4096)
      .Select(culture => new LocaleModel(culture));

    [HttpGet]
    public ActionResult<IEnumerable<LocaleModel>> Get() => Ok(_locales);
  }
}
