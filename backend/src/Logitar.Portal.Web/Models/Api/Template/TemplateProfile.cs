using AutoMapper;
using Logitar.Portal.Core.Emails.Templates.Models;

namespace Logitar.Portal.Web.Models.Api.Template
{
  internal class TemplateProfile : Profile
  {
    public TemplateProfile()
    {
      CreateMap<TemplateModel, TemplateSummary>()
        .ForMember(x => x.UpdatedAt, x => x.MapFrom(y => y.UpdatedAt ?? y.CreatedAt));
    }
  }
}
