using AutoMapper;
using Portal.Core.Emails.Templates.Models;

namespace Portal.Core.Emails.Templates
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
