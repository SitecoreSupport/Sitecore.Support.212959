using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Mvc.ExperienceEditor.Presentation;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Sites;

namespace Sitecore.Support.Mvc.ExperienceEditor.Pipelines.Response.RenderRendering
{
  public class AddWrapper : Sitecore.Mvc.ExperienceEditor.Pipelines.Response.RenderRendering.AddWrapper
  {
    public override void Process([NotNull] RenderRenderingArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      if (args.Rendered)
      {
        return;
      }


      if (!Context.PageMode.IsExperienceEditorEditing)
      {
        return;
      }

      SiteContext site = Context.Site;
      if (site == null)
      {
        return;
      }

      if (!IsDataSourceValid(args))
      {
        return;
      }

      IMarker marker = GetMarker();
      if (marker == null)
      {
        return;
      }

      int index = args.Disposables.FindIndex(x => x.GetType() == typeof(Wrapper));
      if (index < 0)
      {
        index = 0;
      }
      args.Disposables.Insert(index, new Wrapper(args.Writer, marker));
    }

    protected virtual bool IsDataSourceValid(RenderRenderingArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      string dataSource = args.Rendering?.DataSource;
      Database database = args.PageContext?.Item?.Database ?? args.PageContext?.Database;
      if (!IsItemNeedsToBeValidated(database, dataSource)
          || datasourceValidator.IsDatasourceValid(dataSource, database))
      {
        return true;
      }

      Log.Warn($"'{dataSource}' is not valid data source for '{database?.Name}' database or user does not have permissions to access.", this);
      args.AbortPipeline();
      return false;
    }

    protected virtual bool IsItemNeedsToBeValidated([CanBeNull] Database database, [CanBeNull] string dataSource)
    {
      // if this is core DB - it may contain speak renderings, that user does not have permissions to access - we should render them
      return database != null && !string.IsNullOrEmpty(dataSource) && database.Name != Constants.CoreDatabaseName;
    }
  }
}