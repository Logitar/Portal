using AutoMapper;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Infrastructure.Entities;

namespace Logitar.Portal.Infrastructure.Profiles
{
  internal class TemplateProfile : Profile
  {
    public TemplateProfile()
    {
      CreateMap<TemplateEntity, TemplateModel>()
        .IncludeBase<AggregateEntity, AggregateModel>();
    }
  }
}
