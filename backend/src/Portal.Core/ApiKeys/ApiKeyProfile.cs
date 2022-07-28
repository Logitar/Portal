using AutoMapper;
using Portal.Core.ApiKeys.Models;

namespace Portal.Core.ApiKeys
{
  internal class ApiKeyProfile : Profile
  {
    public ApiKeyProfile()
    {
      CreateMap<ApiKey, ApiKeyModel>()
        .IncludeBase<Aggregate, AggregateModel>();
    }
  }
}
