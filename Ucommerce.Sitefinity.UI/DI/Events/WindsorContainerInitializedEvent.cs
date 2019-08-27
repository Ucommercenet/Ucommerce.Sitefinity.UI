using System;
using Castle.Windsor;

namespace Ucommerce.Sitefinity.UI.DI.Events
{
    internal class WindsorContainerInitializedEvent : IWindsorContainerInitializedEvent
    {
        public string Origin { get; set; } = typeof(UcommerceUIModule).Name;

        public IWindsorContainer Container { get; set; }
    }
}
