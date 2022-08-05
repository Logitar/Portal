using AutoMapper;
using Logitar.Portal.Core.Emails.Templates.Models;

namespace Logitar.Portal.Core.Emails.Templates
{
  internal class TemplateProfile : Profile
  {
    public TemplateProfile()
    {
      CreateMap<Template, TemplateModel>()
        .IncludeBase<Aggregate, AggregateModel>();
      CreateMap<TemplateModel, TemplateSummary>()
        .ForMember(x => x.UpdatedAt, x => x.MapFrom(y => y.UpdatedAt ?? y.CreatedAt));
    }
  }
}
