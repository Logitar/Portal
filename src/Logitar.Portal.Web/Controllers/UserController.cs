﻿using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Policies.PortalActor)]
[Route("users")]
public class UserController : Controller
{
  private readonly IUserService _userService;

  public UserController(IUserService userService)
  {
    _userService = userService;
  }

  [HttpGet("/create-user")]
  public ActionResult CreateUser()
  {
    return View(nameof(UserEdit));
  }

  [HttpGet]
  public ActionResult UserList()
  {
    return View();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult> UserEdit(Guid id, CancellationToken cancellationToken = default)
  {
    User? user = await _userService.GetAsync(id, cancellationToken: cancellationToken);
    if (user == null)
    {
      return NotFound();
    }

    return View(user);
  }
}
