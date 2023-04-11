﻿using Logitar.Portal.v2.Contracts.Senders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Constants.Policies.PortalActor)]
[Route("senders")]
public class SenderController : Controller
{
  private readonly ISenderService _senderService;

  public SenderController(ISenderService senderService)
  {
    _senderService = senderService;
  }

  [HttpGet("/create-sender")]
  public ActionResult CreateSender()
  {
    return View(nameof(SenderEdit));
  }

  [HttpGet]
  public ActionResult SenderList()
  {
    return View();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult> SenderEdit(Guid id, CancellationToken cancellationToken = default)
  {
    Sender? sender = await _senderService.GetAsync(id, cancellationToken: cancellationToken);
    if (sender == null)
    {
      return NotFound();
    }

    return View(sender);
  }
}