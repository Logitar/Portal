using AutoMapper;
using Logitar.Portal.Core;
using Logitar.Portal.Core.ApiKeys.Models;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys
{
  internal class ApiKeyProfile : Profile
  {
    public ApiKeyProfile()
    {
      CreateMap<ApiKey, ApiKeyModel>()
        .IncludeBase<Aggregate, AggregateModel>();
      CreateMap<ApiKeyModel, ApiKeySummary>()
        .IncludeBase<AggregateModel, AggregateSummary>();
    }
  }
}
