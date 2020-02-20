using Castle.Windsor;
using Telerik.Sitefinity.Services.Events;

namespace UCommerce.Sitefinity.UI.DI.Events
{
    /// <summary>
    /// The contract to subscribe to the event thrown right after the initialization of the Windsor container.
    /// </summary>
    public interface IWindsorContainerInitializedEvent : IEvent
    {
        IWindsorContainer Container { get; set; }
    }
}
