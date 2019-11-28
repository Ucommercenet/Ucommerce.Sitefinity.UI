using Castle.Windsor;
using Telerik.Sitefinity.Services.Events;

namespace UCommerce.Sitefinity.UI.DI.Events
{
    public interface IWindsorContainerInitializedEvent : IEvent
    {
        IWindsorContainer Container { get; set; }
    }
}
