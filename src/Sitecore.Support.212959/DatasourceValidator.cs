using System;
using System.Linq;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.ParseDataSource;
using Sitecore.Mvc.ExperienceEditor.DatasourceValidator;

namespace Sitecore.Support.Mvc.ExperienceEditor.DatasourceValidator
{
  public class DatasourceValidator : IDatasourceValidator
  {
    public virtual bool IsDatasourceValid([NotNull] string dataSource, [NotNull] Database database)
    {
      try
      {
        if (!IsItem(dataSource, database))
        {
          return ParseDataSourcePipeline.Run(database, dataSource).Any();
        }

        return true;
      }
      catch (Exception ex)
      {
        Log.Warn($"Failed to execute datasource query {ex}", this);
      }

      return false;
    }

    protected virtual bool IsItem([NotNull] string datasource, [NotNull] Database database)
    {
      Assert.ArgumentNotNull(database, nameof(database));
      Assert.ArgumentNotNull(datasource, nameof(datasource));

      string[] dataSourceItems = datasource.Split('|');
      return dataSourceItems.All(it => MainUtil.IsFullPath(it) && database.GetItem(it) != null);
    }
  }
}