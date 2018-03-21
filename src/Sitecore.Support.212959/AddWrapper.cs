// <copyright file="$FileName$" company="Sitecore">Copyright (c) Sitecore. All rights reserved.</copyright>

namespace Sitecore.Mvc.ExperienceEditor.Pipelines.Response.RenderRendering
{
  using Microsoft.Extensions.DependencyInjection;
  using Sitecore.Data;
  using Sitecore.DependencyInjection;
  using Sitecore.Diagnostics;
  using Sitecore.Mvc.ExperienceEditor.DatasourceValidator;
  using Sitecore.Mvc.ExperienceEditor.Extensions;
  using Sitecore.Mvc.ExperienceEditor.Presentation;
  using Sitecore.Mvc.Pipelines.Response.RenderRendering;
  using Sitecore.Mvc.Presentation;
  using Sitecore.Sites;

  /// <summary>
  /// The add wrapper.
  /// </summary>
  public class AddWrapper : RenderRenderingProcessor
  {
    protected readonly IDatasourceValidator datasourceValidator;

    public AddWrapper(IDatasourceValidator datasourceValidator)
    {
      this.datasourceValidator = datasourceValidator;
    }

    public AddWrapper() : this(ServiceLocator.ServiceProvider.GetService<IDatasourceValidator>())
    {
    }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args.
    /// </param>
    public override void Process([NotNull] RenderRenderingArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));
      if (!IsDataSourceValid(args))
      {
        return;
      }

      if (args.Rendered)
      {
        return;
      }

      SiteContext site = Context.Site;
      if (site == null)
      {
        return;
      }

      if (!Context.PageMode.IsExperienceEditorEditing)
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

    /// <summary>
    /// The get marker.
    /// </summary>
    /// <returns>
    /// </returns>
    [CanBeNull]
    protected virtual IMarker GetMarker()
    {
      RenderingContext context = RenderingContext.CurrentOrNull;
      if (context?.Rendering == null)
      {
        return null;
      }

      if (!context.Rendering.IsXmlBasedRendering())
      {
        return null;
      }

      PlaceholderContext placeholderContext = PlaceholderContext.CurrentOrNull;
      if (placeholderContext == null)
      {
        return null;
      }

      return new RenderingMarker(context, placeholderContext);
    }
  }
}