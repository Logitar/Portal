using AutoMapper;
using Logitar.Portal.Core.ApiKeys.Models;

namespace Logitar.Portal.Core.ApiKeys
{
  internal class ApiKeyProfile : Profile
  {
    public ApiKeyProfile()
    {
      CreateMap<ApiKey, ApiKeyModel>()
        .IncludeBase<Aggregate, AggregateModel>();
      CreateMap<ApiKeyModel, ApiKeySummary>()
        .ForMember(x => x.UpdatedAt, x => x.MapFrom(y => y.UpdatedAt ?? y.CreatedAt));
    }
  }
}
