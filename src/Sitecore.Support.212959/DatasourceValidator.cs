// <copyright file="$FileName$" company="Sitecore">Copyright (c) Sitecore. All rights reserved.</copyright>

namespace Sitecore.Mvc.ExperienceEditor.DatasourceValidator
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.ContentSearch;
  using Sitecore.ContentSearch.SearchTypes;
  using Sitecore.ContentSearch.Utilities;
  using Sitecore.Data;
  using Sitecore.Diagnostics;

  public class DatasourceValidator : IDatasourceValidator
  {
    public virtual bool IsDatasourceValid([NotNull] string dataSource, [NotNull] Database database)
    {
      try
      {
        using (IProviderSearchContext context = ContentSearchManager.CreateSearchContext((SitecoreIndexableItem)Context.Item))
        {
          IEnumerable<SearchStringModel> query = SearchStringModel.ParseDatasourceString(dataSource).ToList();

          // In case if query is not valid or contains address/URI of item ParseDatasourceString will return empty collection
          // if pass this empty collection into LinqHelper.CreateQuery it will return all items in Sitecore instance
          if (!query.Any())
          {
            return IsItem(dataSource, database);
          }
          IQueryable<SearchResultItem> result = LinqHelper.CreateQuery<SearchResultItem>(context, query);
          return result.Any();
        }
      }
      catch (Exception ex)
      {
        Log.Warn($"Failed to execute datasource query {ex}", this);
      }

      return false;
    }

    /// <summary>
    /// Checks whether provided datasource is valid item address or ID
    /// </summary>
    /// <param name="datasource">Datasource to check</param>
    /// <param name="database">Database to use</param>
    /// <returns></returns>
    protected virtual bool IsItem([NotNull] string datasource, [NotNull] Database database)
    {
      Assert.ArgumentNotNull(database, nameof(database));
      Assert.ArgumentNotNull(datasource, nameof(datasource));

      string[] dataSourceItems = datasource.Split('|');
      foreach (string dataSourceItem in dataSourceItems)
      {
        if (database.GetItem(dataSourceItem) == null)
        {
          return false;
        }
      }

      return true;
    }
  }
}