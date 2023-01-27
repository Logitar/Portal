using AutoMapper;
using Logitar.Portal.Core;

namespace Logitar.Portal.Application
{
  internal class ErrorProfile : Profile
  {
    public ErrorProfile()
    {
      CreateMap<Error, ErrorModel>()
        .ForMember(x => x.Data, x => x.MapFrom(y => y.Data == null ? null : y.Data.Select(pair => new ErrorDataModel
        {
          Key = pair.Key,
          Value = pair.Value
        })));
    }
  }
}
