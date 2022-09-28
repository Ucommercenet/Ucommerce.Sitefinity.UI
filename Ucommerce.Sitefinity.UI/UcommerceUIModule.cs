using System;
using System.Linq;
using System.Web.Mvc;
using Castle.MicroKernel.Releasers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data.Metadata;
using Telerik.Sitefinity.Metadata.Model;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Services;
using UCommerce.Sitefinity.UI.Mvc;
using UCommerce.Sitefinity.UI.Pages;

namespace UCommerce.Sitefinity.UI
{
    /// <summary>
    /// The UCommerce module class.
    /// </summary>
    public class UCommerceUIModule : ModuleBase
    {
        public const string NAME = "UcommerceUIModule";
        public const string TITLE = "UCommerce UI";
        public const string UCOMMERCE_WIDGET_SECTION = "UCommerce";
        private const string CATALOG_NOT_INDEXED = "Could not find facets document";
        private const string NO_CATALOG_ERROR_MESSAGE = "There is no product catalog configured.";
        private const string NO_CATEGORIES_ERROR_MESSAGE = "There are no product categories configured.";
        private const string RAVEN_SOURCE = "Raven.Database";
        private static volatile IWindsorContainer container;
        public static IWindsorContainer Container => container;
        public override Guid LandingPageId => Guid.Empty;
        public override Type[] Managers => new Type[0];

        public override void Initialize(ModuleSettings settings)
        {
            base.Initialize(settings);
            CreatePageContextField();
            SubscribeToEvents();

            App.WorkWith()
                .Module(settings.Name)
                .Initialize();
        }

        public override void Install(SiteInitializer initializer)
        {
            Log.Write($"Installing the {Title} module");
        }

        public override void Uninstall(SiteInitializer initializer)
        {
            base.Uninstall(initializer);
            DeletePageContextField();
            Log.Write($"Successfully uninstalled {TITLE} module", ConfigurationPolicy.Trace);
        }

        public override void Unload()
        {
            UnsubscribeFromEvents();
            base.Unload();
        }

        protected override ConfigSection GetModuleConfig()
        {
            return null;
        }

        private void Bootstrapper_Bootstrapped(object sender, EventArgs e)
        {
            if (!GlobalFilters.Filters.Any(f => f.GetType() == typeof(ContextResolverAttribute)))
            {
                GlobalFilters.Filters.Add(new ContextResolverAttribute());
            }
        }

        private void CreatePageContextField()
        {
            try
            {
                var metaManager = MetadataManager.GetManager();

                if (metaManager.GetMetaType(typeof(PageNode)) == null)
                {
                    metaManager.CreateMetaType(typeof(PageNode));
                    metaManager.SaveChanges();
                }

                var pageMetaInfo = metaManager.GetMetaType(typeof(PageNode));
                if (!pageMetaInfo.Fields.Any(f => f.FieldName == HttpContextExtensions.PAGE_CONTEXT_FIELD_NAME))
                {
                    var field = metaManager.CreateMetafield(HttpContextExtensions.PAGE_CONTEXT_FIELD_NAME);
                    field.MetaAttributes.Add(new MetaFieldAttribute
                    {
                        Name = "UserFriendlyDataType",
                        Value = "LongText",
                    });

                    field.ClrType = typeof(Lstring).FullName;
                    field.Hidden = true;
                    field.IsLocalizable = true;

                    metaManager.Provider.SuppressSecurityChecks = true;

                    pageMetaInfo.Fields.Add(field);
                    metaManager.SaveChanges();
                    metaManager.Provider.SuppressSecurityChecks = false;
                }
            }
            catch (Exception ex)
            {
                Log.Write(
                    $"Could not install {HttpContextExtensions.PAGE_CONTEXT_FIELD_NAME} field. The following exception has occcured: {Environment.NewLine} {ex}",
                    ConfigurationPolicy.ErrorLog);
            }
        }

        private void DeletePageContextField()
        {
            try
            {
                var metaManager = MetadataManager.GetManager();
                var pageMetaInfo = metaManager.GetMetaType(typeof(PageNode));
                var pageContextField = pageMetaInfo.Fields
                    .Where(f => f.FieldName == HttpContextExtensions.PAGE_CONTEXT_FIELD_NAME)
                    .SingleOrDefault();

                if (pageContextField != null)
                {
                    metaManager.Provider.SuppressSecurityChecks = true;
                    metaManager.Delete(pageContextField);

                    metaManager.SaveChanges();
                    metaManager.Provider.SuppressSecurityChecks = false;
                }
            }
            catch (Exception ex)
            {
                Log.Write(
                    $"Could not delete {HttpContextExtensions.PAGE_CONTEXT_FIELD_NAME} field. The following exception has occcured: {Environment.NewLine} {ex}",
                    ConfigurationPolicy.ErrorLog);
            }
        }

        private void SubscribeToEvents()
        {
            Bootstrapper.Bootstrapped -= Bootstrapper_Bootstrapped;
            Bootstrapper.Bootstrapped += Bootstrapper_Bootstrapped;
        }

        private void UnsubscribeFromEvents()
        {
            Bootstrapper.Bootstrapped -= Bootstrapper_Bootstrapped;
        }

        public static void Register()
        {
            var isModuleInstalled = Config.Get<SystemConfig>()
                .ApplicationModules.Elements
                .Any(m => m.Name.Equals(NAME));

            if (!isModuleInstalled)
            {
                try
                {
                    var configManager = ConfigManager.GetManager();
                    var modulesConfig = configManager.GetSection<SystemConfig>()
                        .ApplicationModules;
                    if (modulesConfig != null)
                    {
                        modulesConfig.Add(NAME,
                            new AppModuleSettings(modulesConfig)
                            {
                                Name = NAME,
                                Title = TITLE,
                                Type = typeof(UCommerceUIModule).AssemblyQualifiedName,
                                Description = "A module class used to encapsulate the UCommerce SDK for Sitefinity",
                                StartupType = StartupType.OnApplicationStart,
                            });

                        configManager.Provider.SuppressSecurityChecks = true;
                        configManager.SaveSection(modulesConfig.Section);
                        configManager.Provider.SuppressSecurityChecks = false;
                    }
                }
                catch (Exception ex)
                {
                    Log.Write($"Could not register the {NAME}. The following exception was encountered:" + Environment.NewLine + ex);
                }
            }
        }

        public static bool TryHandleSystemError(Exception exception, out ActionResult actionResult)
        {
            var result = false;
            actionResult = new ContentResult();
            var systemErrorMessage = string.Empty;
            Log.Write($"Unhandled Exception was thrown. Full exception below: {Environment.NewLine} {exception}", ConfigurationPolicy.ErrorLog);

            if (exception is InvalidOperationException && exception.Message != null)
            {
                if (exception.Message.Contains(RAVEN_SOURCE))
                {
                    systemErrorMessage =
                        "There was an error processing the product facets configuration in Raven DB. Please delete the uCommerce file in  App_Data\\RavenDatabases folder and run the scratch indexer.";
                }
                else if (exception.Message.Contains(NO_CATALOG_ERROR_MESSAGE))
                {
                    systemErrorMessage = "There are no product catalogs configured for your site. Please configure a product catalog.";
                }
                else if (exception.Message.Contains(NO_CATEGORIES_ERROR_MESSAGE))
                {
                    systemErrorMessage = "There are no product categories configured for your site. Please configure a product category.";
                }
                else if (exception.Message.Contains(CATALOG_NOT_INDEXED))
                {
                    systemErrorMessage =
                        "There was an error processing the product facets configuration in Raven DB. Please delete the uCommerce file in  App_Data\\RavenDatabases folder and run the scratch indexer.";
                }

                if (!string.IsNullOrEmpty(systemErrorMessage))
                {
                    result = true;
                    actionResult = new ContentResult
                    {
                        Content = systemErrorMessage
                    };
                }
            }

            return result;
        }

        internal static void InitializeContainer()
        {
            var windsorContainer = new WindsorContainer();
#pragma warning disable CS0618
            windsorContainer.Kernel.ReleasePolicy = new NoTrackingReleasePolicy();
#pragma warning restore CS0618

            var widgetAssemblies = ControllerContainerResolver.RetrieveAssemblies();
            foreach (var assembly in widgetAssemblies)
            {
                windsorContainer.Install(FromAssembly.Instance(assembly));
            }

            container = windsorContainer;
        }

        internal static void RegisterControllerFactory()
        {
            ObjectFactory.Container.RegisterInstance(
                typeof(ISitefinityControllerFactory),
                typeof(WindsorControllerFactory).Name,
                new WindsorControllerFactory(Container),
                new ContainerControlledLifetimeManager());

            var factory = ObjectFactory.Resolve<ISitefinityControllerFactory>(typeof(WindsorControllerFactory).Name);
            ControllerBuilder.Current.SetControllerFactory(factory);
        }
    }
}
