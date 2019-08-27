using Castle.Windsor;
using Telerik.Sitefinity.Services.Events;

namespace Ucommerce.Sitefinity.UI.DI.Events
{
    public interface IWindsorContainerInitializedEvent : IEvent
    {
        IWindsorContainer Container { get; set; }
    }
}
