using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Sitecore.Support.Mvc.ExperienceEditor.DependencyInjection
{
  public class MvcEditorServiceConfigurator : IServicesConfigurator
  {
    public void Configure(IServiceCollection serviceCollection)
    {
      serviceCollection.AddTransient<Sitecore.Mvc.ExperienceEditor.DatasourceValidator.IDatasourceValidator, Sitecore.Support.Mvc.ExperienceEditor.DatasourceValidator.DatasourceValidator>();
    }
  }
}