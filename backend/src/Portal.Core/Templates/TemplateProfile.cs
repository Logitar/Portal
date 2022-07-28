using AutoMapper;
using Portal.Core.Templates.Models;

namespace Portal.Core.Templates
{
  internal class TemplateProfile : Profile
  {
    public TemplateProfile()
    {
      CreateMap<Template, TemplateModel>()
        .IncludeBase<Aggregate, AggregateModel>();
    }
  }
}
