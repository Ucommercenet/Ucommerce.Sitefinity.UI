using System;
using System.ComponentModel;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using Castle.Windsor;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Services;
using Ucommerce.Sitefinity.UI.DI.Events;
using Ucommerce.Sitefinity.UI.Mvc.Infrastructure;

namespace Ucommerce.Sitefinity.UI.App_Start
{
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
                UcommerceUIModule.Register();
                UcommerceUIModule.InitializeContainer();
            }
        }

        private static void Bootstrapper_Bootstrapped(object sender, EventArgs e)
        {
            UcommerceUIModule.RegisterControllerFactory();

            EventHub.Raise(new WindsorContainerInitializedEvent
            {
                Container = UcommerceUIModule.Container
            });

            //Use the following to register the BundleConfig
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.VirtualPathProvider = HostingEnvironment.VirtualPathProvider;
            BundleTable.EnableOptimizations = true;
        }
    }
}
