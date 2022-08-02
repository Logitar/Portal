using AutoMapper;
using Logitar.Portal.Core;
using Logitar.Portal.Core.ApiKeys;
using Logitar.Portal.Core.ApiKeys.Models;
using Logitar.Portal.Core.ApiKeys.Payloads;
using Logitar.Portal.Web.Models.Api.ApiKey;
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
    private readonly IMapper _mapper;

    public ApiKeyApiController(IApiKeyService apiKeyService, IMapper mapper)
    {
      _apiKeyService = apiKeyService;
      _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<ApiKeyModel>> CreateAsync([FromBody] CreateApiKeyPayload payload, CancellationToken cancellationToken)
    {
      ApiKeyModel apiKey = await _apiKeyService.CreateAsync(payload, cancellationToken);
      var uri = new Uri($"/api/keys/{apiKey.Id}", UriKind.Relative);

      return Created(uri, apiKey);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiKeyModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
      return Ok(await _apiKeyService.DeleteAsync(id, cancellationToken));
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<ApiKeySummary>>> GetAsync(bool? isExpired, string? search,
      ApiKeySort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken = default)
    {
      ListModel<ApiKeyModel> apiKeys = await _apiKeyService.GetAsync(isExpired, search,
        sort, desc,
        index, count,
        cancellationToken);

      return Ok(ListModel<ApiKeySummary>.From(apiKeys, _mapper));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiKeyModel>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      ApiKeyModel? apiKey = await _apiKeyService.GetAsync(id, cancellationToken);
      if (apiKey == null)
      {
        return NotFound();
      }

      return Ok(apiKey);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiKeyModel>> UpdateAsync(Guid id, [FromBody] UpdateApiKeyPayload payload, CancellationToken cancellationToken)
    {
      return Ok(await _apiKeyService.UpdateAsync(id, payload, cancellationToken));
    }
  }
}
