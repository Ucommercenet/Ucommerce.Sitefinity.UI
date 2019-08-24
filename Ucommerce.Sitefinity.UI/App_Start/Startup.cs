using System;
using System.ComponentModel;
using System.Web.Mvc;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Mvc;
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
            }
        }

        private static void Bootstrapper_Bootstrapped(object sender, EventArgs e)
        {
            var containerBootstrapper = CastleWindsorBootstrapper.Bootstrap();

            ObjectFactory.Container.RegisterInstance(
                typeof(ISitefinityControllerFactory),
                typeof(CastleWindsorControllerFactory).Name,
                new CastleWindsorControllerFactory(containerBootstrapper.Container),
                new ContainerControlledLifetimeManager());

            var factory = ObjectFactory.Resolve<ISitefinityControllerFactory>(typeof(CastleWindsorControllerFactory).Name);
            ControllerBuilder.Current.SetControllerFactory(factory);
        }
    }
}
