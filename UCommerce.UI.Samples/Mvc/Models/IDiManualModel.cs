namespace Ucommerce.UI.Samples.Mvc.Models
{
    /// <summary>
    /// An interface that is registered manually in the Ucommerce DI container on container initialized event.
    /// </summary>
    public interface IDiManualModel
    {
        DependencyInjectionViewModel GetViewModel();
    }
}
