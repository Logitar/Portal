using AutoMapper;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Infrastructure.Entities;

namespace Logitar.Portal.Infrastructure.Profiles
{
  internal class ApiKeyProfile : Profile
  {
    public ApiKeyProfile()
    {
      CreateMap<ApiKeyEntity, ApiKeyModel>()
        .IncludeBase<AggregateEntity, AggregateModel>();
    }
  }
}
