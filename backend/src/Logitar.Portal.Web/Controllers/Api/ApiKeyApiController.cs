using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/keys")]
  public class ApiKeyApiController : ControllerBase
  {
    private readonly IApiKeyService _apiKeyService;

    public ApiKeyApiController(IApiKeyService apiKeyService)
    {
      _apiKeyService = apiKeyService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiKeyModel>> CreateAsync([FromBody] CreateApiKeyPayload payload, CancellationToken cancellationToken)
    {
      ApiKeyModel apiKey = await _apiKeyService.CreateAsync(payload, cancellationToken);
      Uri uri = new($"/api/keys/{apiKey.Id}", UriKind.Relative);

      return Created(uri, apiKey);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
    {
      await _apiKeyService.DeleteAsync(id, cancellationToken);

      return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<ApiKeyModel>>> GetAsync(DateTime? expiredOn, string? search,
      ApiKeySort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken = default)
    {
      return Ok(await _apiKeyService.GetAsync(expiredOn, search,
        sort, desc,
        index, count,
        cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiKeyModel>> GetAsync(string id, CancellationToken cancellationToken)
    {
      ApiKeyModel? apiKey = await _apiKeyService.GetAsync(id, cancellationToken);
      if (apiKey == null)
      {
        return NotFound();
      }

      return Ok(apiKey);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiKeyModel>> UpdateAsync(string id, [FromBody] UpdateApiKeyPayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _apiKeyService.UpdateAsync(id, payload, cancellationToken));
    }
  }
}
