using Logitar.Portal.Contracts.ApiKeys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Authorize]
[Route("api-keys")]
public class ApiKeyController : ControllerBase
{
  private readonly IApiKeyService _apiKeyService;

  public ApiKeyController(IApiKeyService apiKeyService)
  {
    _apiKeyService = apiKeyService;
  }

  [HttpPost("authenticate")]
  public async Task<ActionResult<ApiKey>> AuthenticateAsync([FromBody] AuthenticateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _apiKeyService.AuthenticateAsync(payload, cancellationToken));
  }

  [HttpPost]
  public async Task<ActionResult<ApiKey>> CreateAsync([FromBody] CreateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    ApiKey apiKey = await _apiKeyService.CreateAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api-keys/{apiKey.Id}"); // TODO(fpion): refactor

    return Created(uri, apiKey);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<ApiKey>> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await _apiKeyService.DeleteAsync(id, cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ApiKey>> ReadAsync(string id, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await _apiKeyService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ApiKey>> ReplaceAsync(string id, [FromBody] ReplaceApiKeyPayload payload, long? version, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await _apiKeyService.ReplaceAsync(id, payload, version, cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<ApiKey>> UpdateAsync(string id, [FromBody] UpdateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await _apiKeyService.UpdateAsync(id, payload, cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }
}
