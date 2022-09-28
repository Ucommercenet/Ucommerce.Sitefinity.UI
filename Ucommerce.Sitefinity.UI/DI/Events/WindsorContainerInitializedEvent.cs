using Castle.Windsor;

namespace UCommerce.Sitefinity.UI.DI.Events
{
    /// <summary>
    /// The class used to fire the event thrown right after the initialization of the Windsor container.
    /// </summary>
    internal class WindsorContainerInitializedEvent : IWindsorContainerInitializedEvent
    {
        public IWindsorContainer Container { get; set; }
        public string Origin { get; set; } = typeof(UCommerceUIModule).Name;
    }
}
