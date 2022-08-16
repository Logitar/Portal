using AutoMapper;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Emails.Templates.Models;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Emails.Templates;

namespace Logitar.Portal.Application.Emails.Templates
{
  internal class TemplateProfile : Profile
  {
    public TemplateProfile()
    {
      CreateMap<Template, TemplateModel>()
        .IncludeBase<Aggregate, AggregateModel>();
      CreateMap<TemplateModel, TemplateSummary>()
        .IncludeBase<AggregateModel, AggregateSummary>();
    }
  }
}
