using System;
using Castle.Windsor;

namespace UCommerce.Sitefinity.UI.DI.Events
{
    internal class WindsorContainerInitializedEvent : IWindsorContainerInitializedEvent
    {
        public string Origin { get; set; } = typeof(UCommerceUIModule).Name;

        public IWindsorContainer Container { get; set; }
    }
}
