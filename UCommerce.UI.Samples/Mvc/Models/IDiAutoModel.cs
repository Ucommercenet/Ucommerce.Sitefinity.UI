namespace Ucommerce.UI.Samples.Mvc.Models
{
    /// <summary>
    /// An interface that is registered automatically in the Ucommerce DI container on application start.
    /// </summary>
    public interface IDiAutoModel
    {
        DependencyInjectionViewModel GetViewModel();
    }
}
