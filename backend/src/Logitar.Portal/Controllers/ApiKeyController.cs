using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Extensions;
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

  [HttpPost]
  public async Task<ActionResult<ApiKey>> CreateAsync([FromBody] CreateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    ApiKey apiKey = await _apiKeyService.CreateAsync(payload, cancellationToken);
    return Created(BuildLocation(apiKey), apiKey);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<ApiKey>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await _apiKeyService.DeleteAsync(id, cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ApiKey>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await _apiKeyService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  private Uri BuildLocation(ApiKey apiKey) => HttpContext.BuildLocation("keys/{id}", new Dictionary<string, string> { ["id"] = apiKey.Id.ToString() });
}
