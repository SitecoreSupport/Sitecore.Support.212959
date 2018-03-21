using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Mvc.ExperienceEditor.DatasourceValidator;
using System;

namespace Sitecore.Mvc.ExperienceEditor.DependencyInjection
{
  public class MvcEditorServiceConfigurator : IServicesConfigurator
  {
    public void Configure(IServiceCollection serviceCollection)
    {
      serviceCollection.AddTransient<IDatasourceValidator, DatasourceValidator>();
    }
  }
}
