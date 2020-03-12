using System;
using System.ComponentModel;
using System.Web.Hosting;
using System.Web.Optimization;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using UCommerce.Sitefinity.UI.DI.Events;
using UCommerce.Sitefinity.UI.Mvc;
using UCommerce.Sitefinity.UI.Resources;

namespace UCommerce.Sitefinity.UI.App_Start
{
    /// <summary>
    /// This class is the entry-level point for the UCommerce UI Module and it handles the initialization of the infrastructure related to the module.
    /// </summary>
    public static class Startup
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void OnApplicationStart()
        {
            Bootstrapper.Initialized -= Bootstrapper_Initialized;
            Bootstrapper.Initialized += Bootstrapper_Initialized;

            Bootstrapper.Bootstrapped -= Bootstrapper_Bootstrapped;
            Bootstrapper.Bootstrapped += Bootstrapper_Bootstrapped;
        }

        private static void Bootstrapper_Initialized(object sender, ExecutedEventArgs e)
        {
            if (e.CommandName == "RegisterRoutes")
            {
                UCommerceUIModule.Register();
                UCommerceUIModule.InitializeContainer();
            }
        }

        private static void Bootstrapper_Bootstrapped(object sender, EventArgs e)
        {
            UCommerceUIModule.RegisterControllerFactory();

            EventHub.Raise(new WindsorContainerInitializedEvent
            {
                Container = UCommerceUIModule.Container
            });

            //Custom resources
            Res.RegisterResource<UcommerceResources>();

            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.VirtualPathProvider = HostingEnvironment.VirtualPathProvider;
            BundleTable.EnableOptimizations = true;

            ObjectFactory.Container.RegisterType<PageEditorRouteHandler, UCommerceMvcPageEditorRouteHandler>();
            ObjectFactory.Container.RegisterType<TemplateEditorRouteHandler, UCommerceMvcTemplateEditorRouteHandler>();
        }
    }
}
