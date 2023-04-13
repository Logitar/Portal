using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Web.Commands;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api;

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
