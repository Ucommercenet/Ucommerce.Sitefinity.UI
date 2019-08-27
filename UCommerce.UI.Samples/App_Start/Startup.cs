using System;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Services;
using Ucommerce.Sitefinity.UI.DI.Events;
using Ucommerce.UI.Samples.Mvc.Models;

namespace Ucommerce.UI.Samples.App_Start
{
    public static class Startup
    {
        public static void OnApplicationStart()
        {
            Bootstrapper.Initialized -= Bootstrapper_Initialized;
            Bootstrapper.Initialized += Bootstrapper_Initialized;
        }

        private static void Bootstrapper_Initialized(object sender, EventArgs e)
        {
            EventHub.Unsubscribe<IWindsorContainerInitializedEvent>(WindsorContainer_Initialized_Handler);
            EventHub.Subscribe<IWindsorContainerInitializedEvent>(WindsorContainer_Initialized_Handler);
        }

        public static void WindsorContainer_Initialized_Handler(IWindsorContainerInitializedEvent eventInfo)
        {
            var container = eventInfo.Container;

            container.Register(
                 Castle.MicroKernel.Registration.Component
                 .For<IDiManualModel>()
                 .ImplementedBy<DiManualModel>()
                 .LifestylePerWebRequest());
        }
    }
}
