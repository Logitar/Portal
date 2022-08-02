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
    }
  }
}
