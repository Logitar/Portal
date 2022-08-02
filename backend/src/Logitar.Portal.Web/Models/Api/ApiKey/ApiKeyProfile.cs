using AutoMapper;
using Logitar.Portal.Core.ApiKeys.Models;

namespace Logitar.Portal.Web.Models.Api.ApiKey
{
  internal class ApiKeyProfile : Profile
  {
    public ApiKeyProfile()
    {
      CreateMap<ApiKeyModel, ApiKeySummary>()
        .ForMember(x => x.UpdatedAt, x => x.MapFrom(y => y.UpdatedAt ?? y.CreatedAt));
    }
  }
}
