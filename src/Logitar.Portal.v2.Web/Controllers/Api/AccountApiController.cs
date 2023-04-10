using Logitar.Portal.v2.Contracts.Sessions;
using Logitar.Portal.v2.Web.Commands;
using Logitar.Portal.v2.Web.Extensions;
using Logitar.Portal.v2.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers.Api;

[ApiController]
[Route("api/account")]
public class AccountApiController : ControllerBase
{
  private readonly IMediator _mediator;

  public AccountApiController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult> SignInAsync([FromBody] PortalSignInInput input, CancellationToken cancellationToken)
  {
    Session session = await _mediator.Send(new PortalSignIn(input), cancellationToken);
    HttpContext.SignIn(session);

    return NoContent();
  }
}
